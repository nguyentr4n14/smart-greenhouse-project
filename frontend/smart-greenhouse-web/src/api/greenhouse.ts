export type Reading = {
  id: number;
  deviceId: number; // FK -> Device.Id
  sensorType: 'Temperature' | 'Humidity' | 'Light' | 'SoilMoisture'; // Enum values from backend
  value: number;
  unit: string; // °C|%|lux|%
  timestamp: string; // ISO-8601
};

/**
 * Fetch readings, optionally filtered by deviceId and sensorType.
 */
export async function fetchReadings(params?: {
  deviceId?: number;
  sensorType?: string;
}) {
  const search = new URLSearchParams();
  if (params?.deviceId !== undefined)
    search.set('deviceId', String(params.deviceId));
  if (params?.sensorType) search.set('sensorType', params.sensorType);

  const url = `/api/readings${
    search.toString() ? `?${search.toString()}` : ''
  }`;
  const res = await fetch(url);
  if (!res.ok) throw new Error(`Failed to load readings: ${res.status}`);
  return (await res.json()) as Reading[];
}
