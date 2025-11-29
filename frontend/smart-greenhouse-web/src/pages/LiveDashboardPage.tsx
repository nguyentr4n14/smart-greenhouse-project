import { useLiveReadings } from '../hooks/useLiveReadings';
import LiveReadingsPanel from '../components/LiveReadingsPanel';

const WS_URL = 'ws://localhost:5080/ws/live-readings';

export default function LiveDashboardPage() {
  const { readings, status, error, clearReadings } = useLiveReadings(WS_URL);

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="max-w-7xl mx-auto">
        <header className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">
            🌿 Smart Greenhouse - Live Dashboard
          </h1>
          <p className="text-gray-600">
            Real-time sensor data from ESP32 devices via MQTT → Backend → WebSocket
          </p>
        </header>

        <div className="mb-6 bg-blue-50 border border-blue-200 rounded-lg p-4">
          <h3 className="font-semibold text-blue-900 mb-2">How it works:</h3>
          <ol className="list-decimal list-inside text-sm text-blue-800 space-y-1">
            <li>ESP32 devices publish sensor data to the embedded MQTT broker (port 1883)</li>
            <li>Backend receives MQTT messages and processes them via Esp32MessageHandler</li>
            <li>Readings are saved to the database and mapped to DTOs</li>
            <li>Updates are broadcast to this frontend via WebSocket in real-time</li>
            <li>This dashboard automatically displays new readings as they arrive</li>
          </ol>
        </div>

        <LiveReadingsPanel
          readings={readings}
          status={status}
          error={error}
          onClear={clearReadings}
        />
      </div>
    </div>
  );
}
