import { useEffect, useState } from "react";

export function useSingleParkingWebSocket(id) {
  const [liveParking, setLiveParking] = useState(null);

  useEffect(() => {
    if (!id) return;

    const ws = new WebSocket(`ws://localhost:5077/ws/singleparking?id=${id}`);

    ws.onopen = () => {
      //console.log("WebSocket connected");
    };

    ws.onmessage = (event) => {
        try {
            const data = JSON.parse(event.data);
            if (data?.Id === Number(id)) {
            setLiveParking({
                freeSpotsCount: data.FreeSpotsCount,
                occupancy: data.Occupancy,
            });
            }
        } catch (e) {
            //console.error("Neispravan WebSocket podatak", e);
        }
        };


    ws.onerror = (err) => {
      //console.error("WebSocket error:", err);
    };

    ws.onclose = (event) => {
      //console.log(`WebSocket closed: code=${event.code}, reason=${event.reason}`);
    };

    return () => {
      ws.close();
    };
  }, [id]);

  return liveParking;
}
