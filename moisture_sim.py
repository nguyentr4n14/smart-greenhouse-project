import json
import random
import time
import socket
import sys
from typing import Tuple

import paho.mqtt.client as mqtt

# ====== Configuration (adjust as needed) ======
SSID = "ssid"                       # not used in sim, just kept for reference
WIFI_PASSWORD = "pw"          # not used in sim

MQTT_SERVER = "192.168.0.208"         # MQTT server address (backend/server app)
MQTT_PORT = 1883

DEVICE_NAME = "GreenhousePi"        # must match DeviceName in backend
SENSOR_TYPE = "SoilMoisture"        # maps to SensorTypeEnum.SoilMoisture

# Simulated ADC calibration
WET = 1500
DRY = 3600

PUBLISH_INTERVAL = 10.0             # seconds (ESP code uses 10000 ms)
CLIENT_ID_PREFIX = "ESP32-Moisture-Sim-"

# Optional: how "smooth" the random values are
RAW_MIN = WET
RAW_MAX = DRY
RAW_JITTER = 80                     # max change per publish step

# ====== MQTT callbacks ======
def on_connect(client: mqtt.Client, userdata, flags, rc):
    if rc == 0:
        print("MQTT: Connected successfully")
    else:
        print(f"MQTT: Connection failed with code {rc}")


def on_disconnect(client: mqtt.Client, userdata, rc):
    print(f"MQTT: Disconnected (rc={rc})")


# ====== Simulation helpers ======
def compute_moisture(raw: int, wet: int, dry: int) -> float:
    """
    Mimic the ESP32 logic:

        float moisture = 100.0 * (dry - raw) / (dry - wet);
        moisture = constrain(moisture, 0, 100);
    """
    if dry == wet:
        return 0.0

    moisture = 100.0 * (dry - raw) / (dry - wet)
    # Constrain 0–100
    moisture = max(0.0, min(100.0, moisture))
    return moisture


def next_raw_value(prev_raw: int | None) -> int:
    """
    Generate the next raw ADC-like value.
    If prev_raw is None, start somewhere in the middle.
    Otherwise, move a bit randomly around the previous value.
    """
    if prev_raw is None:
        return random.randint(RAW_MIN, RAW_MAX)

    delta = random.randint(-RAW_JITTER, RAW_JITTER)
    value = prev_raw + delta
    value = max(RAW_MIN, min(RAW_MAX, value))
    return value


def build_topic(device_name: str, sensor_type: str) -> str:
    # greenhouse/{deviceName}/sensor/{sensorType}
    return f"greenhouse/{device_name}/sensor/{sensor_type}"


def build_payload(raw: int, moisture: float) -> str:
    """
    Build JSON payload:

        {
          "value": <moisture>,
          "unit": "%",
          "raw": <raw>
        }
    """
    payload = {
        "value": round(moisture, 2),  # similar to float in C++ (2 decimals for readability)
        "unit": "%",
        "raw": raw,
    }
    return json.dumps(payload)


# ====== Main loop ======
def main():
    # Create a client ID similar to the ESP32 code using hostname
    hostname = socket.gethostname()
    client_id = CLIENT_ID_PREFIX + hostname

    client = mqtt.Client(client_id=client_id, clean_session=True)
    client.on_connect = on_connect
    client.on_disconnect = on_disconnect

    print(f"Connecting to MQTT broker {MQTT_SERVER}:{MQTT_PORT} with client ID {client_id} ...")
    client.connect(MQTT_SERVER, MQTT_PORT, keepalive=60)

    # Start background network loop
    client.loop_start()

    topic = build_topic(DEVICE_NAME, SENSOR_TYPE)
    print(f"Using topic: {topic}")

    last_publish = 0.0
    prev_raw = None

    try:
        while True:
            now = time.time()
            if now - last_publish >= PUBLISH_INTERVAL:
                last_publish = now

                # Simulate raw reading
                raw = next_raw_value(prev_raw)
                prev_raw = raw

                moisture = compute_moisture(raw, WET, DRY)

                print(f"Raw: {raw}  Moisture %: {moisture:.2f}")

                payload = build_payload(raw, moisture)
                print(f"Publishing to {topic}: {payload}")

                result = client.publish(topic, payload)
                status = result[0]

                if status == mqtt.MQTT_ERR_SUCCESS:
                    print("MQTT publish ok")
                else:
                    print(f"MQTT publish failed with status {status}")

            # Sleep a bit to avoid tight loop
            time.sleep(0.1)

    except KeyboardInterrupt:
        print("\nStopping simulator...")

    finally:
        client.loop_stop()
        client.disconnect()
        print("MQTT client disconnected")


if __name__ == "__main__":
    # Optional small CLI argument override:
    # e.g. `python esp32_moisture_sim.py GreenhousePi SoilMoisture`
    if len(sys.argv) >= 2:
        DEVICE_NAME = sys.argv[1]
    if len(sys.argv) >= 3:
        SENSOR_TYPE = sys.argv[2]

    main()