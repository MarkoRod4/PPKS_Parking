import { useEffect, useState } from "react";

export function useSingleParkingWebSocket(initial, id) {
  const [liveParking, setLiveParking] = useState(initial);

  useEffect(() => {
    if (!id) return;

    const ws = new WebSocket(`ws://localhost:5077/ws/singleparking?id=${id}`);

    ws.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);

        if (data?.id === id) {
          setLiveParking(prev => ({
            ...prev,
            freeSpots: data.freeSpots,
            occupancy: data.occupancy
          }));
        }
      } catch (e) {
        console.error("Neispravan WebSocket podatak", e);
      }
    };

    return () => ws.close();
  }, [id]);

  return liveParking;
}
