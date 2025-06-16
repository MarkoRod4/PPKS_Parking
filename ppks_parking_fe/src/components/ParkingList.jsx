import React from "react";
import { Link } from "react-router-dom";
import "./ParkingList.css";

const getOccupancyClass = (occupancy) => {
  if (occupancy <= 50) return "occupancy-low";
  if (occupancy <= 80) return "occupancy-medium";
  return "occupancy-high";                         
};

const ParkingList = ({ parkings }) => {
  return (
    <div className="parking-list">
      {parkings.map((p) => (
        <Link
          to={`/parking/${p.id}`}
          key={p.id}
          className={`parking-card-link ${getOccupancyClass(p.occupancy)}`}
        >
          <div className="parking-card">
            <h2>{p.name}</h2>
            <p>Slobodna mjesta: {p.freeSpotsCount}</p>
            <p>Popunjenost: {p.occupancy}%</p>
          </div>
        </Link>
      ))}
    </div>
  );
};

export default ParkingList;
