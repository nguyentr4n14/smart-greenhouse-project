import type { LiveReading } from '../hooks/useLiveReadings';

type Props = {
  readings: LiveReading[];
  status: 'connecting' | 'connected' | 'disconnected';
  error: string | null;
  onClear: () => void;
};

export default function LiveReadingsPanel({ readings, status, error, onClear }: Props) {
  const statusColor = {
    connecting: 'bg-yellow-500',
    connected: 'bg-green-500',
    disconnected: 'bg-red-500',
  }[status];

  const statusText = {
    connecting: 'Connecting...',
    connected: 'Live',
    disconnected: 'Disconnected',
  }[status];

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-3">
          <h2 className="text-2xl font-bold text-gray-800">Live Sensor Readings</h2>
          <div className="flex items-center gap-2">
            <div className={`w-3 h-3 rounded-full ${statusColor} animate-pulse`} />
            <span className="text-sm font-medium text-gray-600">{statusText}</span>
          </div>
        </div>
        <button
          onClick={onClear}
          className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
        >
          Clear
        </button>
      </div>

      {error && (
        <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
          {error}
        </div>
      )}

      {readings.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          <p className="text-lg">Waiting for sensor data...</p>
          <p className="text-sm mt-2">Make sure your ESP32 devices or simulator are running.</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Time
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Device
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Sensor Type
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Value
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {readings.map((reading, index) => (
                <tr key={`${reading.id}-${index}`} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {new Date(reading.timestamp).toLocaleTimeString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {reading.deviceName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                    {reading.sensorType}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-mono text-blue-600">
                    {reading.value.toFixed(2)} {reading.unit}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {readings.length > 0 && (
        <div className="mt-4 text-sm text-gray-500 text-right">
          Showing {readings.length} reading{readings.length !== 1 ? 's' : ''}
        </div>
      )}
    </div>
  );
}
