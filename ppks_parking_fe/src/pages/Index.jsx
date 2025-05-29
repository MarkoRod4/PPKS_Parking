import React, { useEffect, useState } from "react";
import { fetchParkings } from "../api/parking";
import { useParkingWebSocket } from "../hooks/useParkingWebSocket";
import ParkingList from "../components/ParkingList";

const Index = () => {
  const [initialData, setInitialData] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchParkings()
      .then(data => {
        const values = data?.$values ?? [];
        const enriched = values.map(p => ({
          id: p.id,
          name: p.name,
          freeSpotsCount: p.freeSpotsCount
        }));
        console.log(enriched);
        setInitialData(enriched);
        setLoading(false);
      })
      .catch(err => {
        console.error("Greška:", err);
        setLoading(false);
      });
  }, []);

 const parkings = useParkingWebSocket(initialData);

  if (loading) return <p>Učitavanje...</p>;

  return (
    <div className="p-4">
      <h1 className="text-2xl mb-4">Pregled parkinga</h1>
      <ParkingList parkings={parkings} />
    </div>
  );
};

export default Index;
