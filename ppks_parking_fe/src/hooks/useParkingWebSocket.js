import { useEffect, useState } from 'react';

export function useParkingWebSocket(initialData = []) {
  const [parkingData, setParkingData] = useState(initialData);
  const [isLive, setIsLive] = useState(true);

  useEffect(() => {
    const socket = new WebSocket('ws://localhost:5077/ws/parking');
    
    socket.onopen = () => {
      console.log('✅ WebSocket konekcija otvorena.');
      setIsLive(true);
    };

    socket.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);

        console.log("🔄 WebSocket data:", data); // 👈 Dodaj ovo!

        // Ako dobiješ direktno listu parkinga, možeš je direktno setati
        const updated = data?.$values ?? data ?? [];
        const parsed = data.map(p => ({
          id: p.Id,
          name: p.Name,
          freeSpotsCount: p.FreeSpotsCount
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
