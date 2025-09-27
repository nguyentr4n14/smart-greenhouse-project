namespace SmartGreenhouse.Domain.Enums;

/// <summary>
/// Enum representing different types of device integration methods in the smart greenhouse system.
/// </summary>
public enum DeviceTypeEnum
{
    /// <summary>
    /// Simulated device - generates fake values for testing and development.
    /// No hardware required, runs in-process.
    /// </summary>
    Simulated = 0,
    
    /// <summary>
    /// MQTT Edge device - communicates via MQTT protocol through a broker.
    /// Typically used for real IoT devices that publish sensor data to an MQTT topic.
    /// </summary>
    MqttEdge = 1
    
    // Future device types can be added here:
    // UsbProbe = 2,     // Direct USB connection to sensor probes
    // ModbusRtu = 3,    // Modbus RTU over serial communication
    // LoRaWan = 4,      // LoRaWAN wireless sensors
}