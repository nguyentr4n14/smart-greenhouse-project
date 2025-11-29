import { useState, useEffect, useCallback } from 'react';

export type LiveReading = {
  id: number;
  deviceId: number;
  deviceName: string;
  sensorType: string;
  value: number;
  unit: string;
  timestamp: string;
};

type ConnectionStatus = 'connecting' | 'connected' | 'disconnected';

export function useLiveReadings(url: string) {
  const [readings, setReadings] = useState<LiveReading[]>([]);
  const [status, setStatus] = useState<ConnectionStatus>('connecting');
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ws: WebSocket | null = null;
    let reconnectTimeout: ReturnType<typeof setTimeout>;

    const connect = () => {
      try {
        setStatus('connecting');
        ws = new WebSocket(url);

        ws.onopen = () => {
          setStatus('connected');
          setError(null);
          console.log('WebSocket connected');
        };

        ws.onmessage = (event) => {
          try {
            const reading: LiveReading = JSON.parse(event.data);
            setReadings((prev) => [reading, ...prev].slice(0, 50)); // Keep last 50 readings
          } catch (err) {
            console.error('Failed to parse WebSocket message:', err);
          }
        };

        ws.onerror = (event) => {
          console.error('WebSocket error:', event);
          setError('WebSocket connection error');
        };

        ws.onclose = () => {
          setStatus('disconnected');
          console.log('WebSocket disconnected, reconnecting in 3s...');
          // Attempt to reconnect after 3 seconds
          reconnectTimeout = setTimeout(connect, 3000);
        };
      } catch (err) {
        console.error('Failed to create WebSocket:', err);
        setError('Failed to connect to WebSocket');
        setStatus('disconnected');
      }
    };

    connect();

    return () => {
      if (reconnectTimeout) {
        clearTimeout(reconnectTimeout);
      }
      if (ws) {
        ws.close();
      }
    };
  }, [url]);

  const clearReadings = useCallback(() => {
    setReadings([]);
  }, []);

  return { readings, status, error, clearReadings };
}
