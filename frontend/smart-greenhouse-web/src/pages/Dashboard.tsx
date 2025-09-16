import { useEffect, useMemo, useState } from 'react';
import { fetchReadings, type Reading } from '../api/greenhouse';
import { fetchDevices, type Device } from '../api/devices';
import { SensorCard } from '../components/SensorCard';

export default function Dashboard() {
    const [devices, setDevices] = useState<Device[]>([]);
    const [selectedDeviceId, setSelectedDeviceId] = useState<
        number | undefined
    >(undefined);
    const [readings, setReadings] = useState<Reading[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    // Load devices on mount
    useEffect(() => {
        fetchDevices()
            .then((d) => {
                setDevices(d);
                // pick first device by default if available
                if (d.length > 0) setSelectedDeviceId(d[0].id);
            })
            .catch((e) => setError(String(e)));
    }, []);

    // Load readings whenever selected device changes
    useEffect(() => {
        if (selectedDeviceId === undefined) {
            setReadings([]);
            setLoading(false);
            return;
        }
        setLoading(true);
        fetchReadings({ deviceId: selectedDeviceId })
            .then((r) => setReadings(r))
            .catch((e) => setError(String(e)))
            .finally(() => setLoading(false));
    }, [selectedDeviceId]);

    const latestByType = useMemo(() => {
        const map = new Map<string, Reading>();
        for (const r of readings) {
            const prev = map.get(r.sensorType);
            if (!prev || prev.timestamp < r.timestamp) map.set(r.sensorType, r);
        }
        return map;
    }, [readings]);

    return (
        <div className="p-6 space-y-6">
            {/* Header + device picker */}
            <div className="flex flex-col sm:flex-row sm:items-center gap-3">
                <h1 className="text-2xl font-bold">Smart Greenhouse</h1>
                <div className="sm:ml-auto">
                    <label className="mr-2 text-sm text-gray-600">Device</label>
                    <select
                        className="border rounded-lg px-3 py-2 bg-white"
                        value={selectedDeviceId ?? ''}
                        onChange={(e) =>
                            setSelectedDeviceId(
                                e.target.value
                                    ? Number(e.target.value)
                                    : undefined
                            )
                        }
                    >
                        {devices.length === 0 && (
                            <option value="">No devices</option>
                        )}
                        {devices.map((d) => (
                            <option key={d.id} value={d.id}>
                                {d.deviceName} (#{d.id})
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            {error && <div className="text-red-600">{error}</div>}
            {devices.length === 0 && (
                <div className="text-gray-600">
                    No devices found. Create one via Swagger (
                    <code>/api/devices</code>) or cURL (see examples below).
                </div>
            )}
            {loading && <div className="text-gray-500">Loading readings…</div>}

            {/* Sensor cards */}
            {!loading && (
                <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
                    <SensorCard
                        title="Temperature"
                        value={latestByType.get('temp')?.value}
                        unit={latestByType.get('temp')?.unit}
                    />
                    <SensorCard
                        title="Humidity"
                        value={latestByType.get('humidity')?.value}
                        unit={latestByType.get('humidity')?.unit}
                    />
                    <SensorCard
                        title="Light"
                        value={latestByType.get('light')?.value}
                        unit={latestByType.get('light')?.unit}
                    />
                    <SensorCard
                        title="Soil Moisture"
                        value={latestByType.get('soilMoisture')?.value}
                        unit={latestByType.get('soilMoisture')?.unit}
                    />
                </div>
            )}
        </div>
    );
}
