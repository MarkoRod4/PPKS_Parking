import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import "./ParkingDetails.css";
import { useSingleParkingWebSocket } from "../hooks/useSingleParkingWebSocket";

const API_BASE = process.env.REACT_APP_API_BASE_URL;

export default function ParkingDetails() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [parking, setParking] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedDay, setSelectedDay] = useState("Monday");

const liveData = useSingleParkingWebSocket(id);

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

  useEffect(() => {
    if (liveData && parking) {
      setParking((prev) => ({
        ...prev,
        freeSpotsCount: liveData.freeSpotsCount,
        occupancy: liveData.occupancy,
      }));
    }
  }, [liveData]);

  if (loading) return <p>Učitavanje...</p>;
  if (error) return <p>Greška: {error}</p>;
  if (!parking) return null;

  const dailyStatsByDay = parking.dailyStats.filter(
    (stat) => stat.day === selectedDay
  );

  const allDays = [
    "Monday", "Tuesday", "Wednesday", "Thursday",
    "Friday", "Saturday", "Sunday",
  ];

  return (
    <div className="details-container">
      <button className="back-button" onClick={() => navigate(-1)}>
        ← Nazad
      </button>

      <div
        className={`parking-card ${
          parking.occupancy < 50
            ? "low-occupancy"
            : parking.occupancy < 80
            ? "medium-occupancy"
            : "high-occupancy"
        }`}
      >
        <h2>{parking.name}</h2>
        <p>Slobodna mjesta: {parking.freeSpotsCount}</p>
        <p>Popunjenost: {parking.occupancy}%</p>
      </div>

      <h2 className="details-heading">Tjedna statistika</h2>
      <table className="details-table">
        <thead>
          <tr>
            <th>Dan</th>
            <th>Prosječna popunjenost</th>
          </tr>
        </thead>
        <tbody>
          {parking.weeklyStats.map((dayStat) => (
            <tr key={dayStat.day}>
              <td>{dayStat.day}</td>
              <td>
                {dayStat.avgOccupiedPercent == null
                  ? "N/A"
                  : `${dayStat.avgOccupiedPercent}%`}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h2 className="details-heading">Dnevna statistika po satima</h2>

      <div className="day-tabs">
        {allDays.map((day) => (
          <button
            key={day}
            onClick={() => setSelectedDay(day)}
            className={`tab-button ${selectedDay === day ? "active" : ""}`}
          >
            {day.slice(0, 3)}
          </button>
        ))}
      </div>

      <table className="details-table">
        <thead>
          <tr>
            <th>Sati</th>
            <th>Prosječna popunjenost</th>
          </tr>
        </thead>
        <tbody>
          {dailyStatsByDay.map((hourStat, idx) => (
            <tr key={idx}>
              <td>{hourStat.hour}:00</td>
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
