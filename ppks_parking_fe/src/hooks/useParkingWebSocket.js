import { useEffect, useState } from 'react';

export function useParkingWebSocket(initialData = []) {
  const [parkingData, setParkingData] = useState(initialData);

  useEffect(() => {
    const socket = new WebSocket('ws://localhost:5077/ws/parking');
    
    socket.onopen = () => {
      console.log('WebSocket konekcija otvorena.');
    };

    socket.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        const parsed = data.map(p => ({
          id: p.Id,
          name: p.Name,
          freeSpotsCount: p.FreeSpotsCount,
          occupancy: p.Occupancy
        }));


        setParkingData(parsed);
      } catch (err) {
        console.error('Greška u parsiranju WebSocket poruke:', err);
      }
    };

    socket.onerror = (err) => {
      console.error('WebSocket error:', err);
    };

    socket.onclose = () => {
      console.warn('WebSocket zatvoren.');
    };

    
    return () => {
      socket.close();
    };
  }, []);

  return parkingData;
}
