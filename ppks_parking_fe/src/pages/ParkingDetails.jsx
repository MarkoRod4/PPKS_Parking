import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";

const API_BASE = process.env.REACT_APP_API_BASE_URL;

export default function ParkingDetails() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [parking, setParking] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedDay, setSelectedDay] = useState("Monday");

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

  // Filtriraj dnevne statistike po odabranom danu
  const dailyStatsByDay = parking.dailyStats.filter(
    (stat) => stat.day === selectedDay
  );

  const allDays = [
    "Monday", "Tuesday", "Wednesday", "Thursday",
    "Friday", "Saturday", "Sunday"
  ];

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
              <td>
                {dayStat.avgOccupiedPercent == null
                  ? "N/A"
                  : `${dayStat.avgOccupiedPercent}%`}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h2 style={{ marginTop: 20 }}>Dnevna statistika po satima</h2>

      <div style={{ marginBottom: 10 }}>
        {allDays.map((day) => (
          <button
            key={day}
            onClick={() => setSelectedDay(day)}
            style={{
              marginRight: 8,
              padding: "5px 10px",
              backgroundColor: day === selectedDay ? "#ccc" : "#f0f0f0",
              border: "1px solid #aaa",
              borderRadius: "4px",
              cursor: "pointer",
            }}
          >
            {day}
          </button>
        ))}
      </div>

      <table border="1" cellPadding="5" style={{ borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th>Sati</th>
            <th>Ukupno zapisa</th>
            <th>Prosječna zauzetost (%)</th>
          </tr>
        </thead>
        <tbody>
          {dailyStatsByDay.map((hourStat, idx) => (
            <tr key={idx}>
              <td>{hourStat.hour}:00</td>
              <td>{hourStat.totalRecords}</td>
              <td>
                {hourStat.avgOccupiedPercent == null
                  ? "N/A"
                  : `${hourStat.avgOccupiedPercent}%`}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
