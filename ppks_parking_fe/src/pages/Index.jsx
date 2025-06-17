import React, { useEffect, useState } from "react";
import { fetchParkings } from "../api/parking";
import { useParkingWebSocket } from "../hooks/useParkingWebSocket";
import ParkingList from "../components/ParkingList.jsx";
import "./index.css";

const Index = () => {
  const [loading, setLoading] = useState(true);
  const [readyData, setReadyData] = useState(null);

  useEffect(() => {
    fetchParkings()
      .then(data => {
        const values = data?.$values ?? [];
        const enriched = values.map(p => ({
          id: p.id,
          name: p.name,
          freeSpotsCount: p.freeSpotsCount,
        }));
        setReadyData(enriched);
        setLoading(false);
      })
      .catch(err => {
        console.error("Greška:", err);
        setLoading(false);
      });
  }, []);

  const parkings = useParkingWebSocket(readyData || []);

  if (loading || !readyData) return <p>Učitavanje...</p>;

  return (
    <div className="index-container">
      <h1 className="index-title">Pregled parkinga</h1>
      <ParkingList parkings={parkings} />
    </div>
  );
};

export default Index;
