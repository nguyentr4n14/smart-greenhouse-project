export type Device = {
  id: number;
  deviceName: string;
  deviceType: 'Simulated' | 'MqttEdge';
  createdAt: string;
};

export async function fetchDevices() {
  const res = await fetch('/api/devices');
  if (!res.ok) throw new Error(`Failed to load devices: ${res.status}`);
  return (await res.json()) as Device[];
}
