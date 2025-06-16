import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";

const API_BASE = process.env.REACT_APP_API_BASE_URL;

export default function ParkingDetails() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [parking, setParking] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchParking() {
      try {
        setLoading(true);
        const res = await fetch(`${API_BASE}/api/parking/${id}`);
        if (!res.ok) throw new Error("Ne mogu dohvatiti podatke");
        const data = await res.json();
        setParking(data);
      } catch (e) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    }
    fetchParking();
  }, [id]);

  if (loading) return <p>Učitavanje...</p>;
  if (error) return <p>Greška: {error}</p>;
  if (!parking) return null;

  return (
    <div>
      <button onClick={() => navigate(-1)} style={{ marginBottom: 20 }}>
        ← Nazad
      </button>

      <h1>{parking.name}</h1>
      <p>Ukupno mjesta: {parking.totalSpots}</p>
      <p>Zauzeto: {parking.occupiedSpots}</p>
      <p>Slobodno: {parking.freeSpots}</p>

      <h2>Tjedna statistika</h2>
      <table border="1" cellPadding="5" style={{ borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th>Dan</th>
            <th>Ukupno zapisa</th>
            <th>Prosječna zauzetost (%)</th>
          </tr>
        </thead>
        <tbody>
          {parking.weeklyStats.map((dayStat) => (
            <tr key={dayStat.day}>
              <td>{dayStat.day}</td>
              <td>{dayStat.totalRecords}</td>
              <td>{dayStat.avgOccupiedPercent.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <h2 style={{ marginTop: 20 }}>Dnevna statistika po satima</h2>
      <table border="1" cellPadding="5" style={{ borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th>Dan</th>
            <th>Sati</th>
            <th>Ukupno zapisa</th>
            <th>Prosječna zauzetost (%)</th>
          </tr>
        </thead>
        <tbody>
          {parking.dailyStats.map((hourStat, idx) => (
            <tr key={idx}>
              <td>{hourStat.day}</td>
              <td>{hourStat.hour}:00</td>
              <td>{hourStat.totalRecords}</td>
              <td>{hourStat.avgOccupiedPercent.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
