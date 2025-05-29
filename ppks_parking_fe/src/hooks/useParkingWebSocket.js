import { useEffect, useState } from "react";

export function useParkingWebSocket(initialData) {
  const [data, setData] = useState(initialData);

  useEffect(() => {
const socket = new WebSocket("ws://localhost:5077/ws/parking");

    socket.onmessage = (event) => {
      try {
        const raw = JSON.parse(event.data);
        const parsed = raw?.$values ?? [];
        setData(parsed);
      } catch (err) {
        console.error("Greška u WebSocket poruci:", err);
      }
    };

    socket.onerror = (event) => {
      console.error("WebSocket error:", event);
    };

    socket.onclose = (event) => {
      console.warn("WebSocket zatvoren:", event);
    };

    return () => {
      socket.close();
    };
  }, []);

  return data;
}
