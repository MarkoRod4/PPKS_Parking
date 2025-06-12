import { useEffect, useState } from 'react';

export function useParkingWebSocket(initialData = []) {
  const [parkingData, setParkingData] = useState(initialData);
  const [isLive, setIsLive] = useState(true);

  useEffect(() => {
    const socket = new WebSocket('wss://localhost:7206/ws/parking');

    socket.onopen = () => {
      console.log('✅ WebSocket konekcija otvorena.');
      setIsLive(true);
    };

    socket.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);

        // Ako dobiješ direktno listu parkinga, možeš je direktno setati
        const updated = data?.$values ?? data ?? [];
        const parsed = updated.map(p => ({
          id: p.id,
          name: p.name,
          freeSpotsCount: p.freeSpotsCount
        }));

        setParkingData(parsed);
      } catch (err) {
        console.error('❌ Greška u parsiranju WebSocket poruke:', err);
      }
    };

    socket.onerror = (err) => {
      console.error('❌ WebSocket error:', err);
      setIsLive(false);
    };

    socket.onclose = () => {
      console.warn('⚠️ WebSocket zatvoren.');
      setIsLive(false);
    };

    return () => {
      socket.close();
    };
  }, []);

  return parkingData;
}
