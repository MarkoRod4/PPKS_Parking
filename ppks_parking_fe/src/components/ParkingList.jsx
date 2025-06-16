import React from "react";
import { Link } from "react-router-dom";

const ParkingList = ({ parkings }) => {
  return (
    <div className="grid gap-4">
      {parkings.map(p => (
        <div key={p.id} className="border p-4 rounded shadow">
          <h2 className="text-xl font-bold">
            <Link to={`/parking/${p.id}`} className="text-blue-600 hover:underline">
              {p.name}
            </Link>
          </h2>
          <p>Slobodna mjesta: {p.freeSpotsCount}</p>
        </div>
      ))}
    </div>
  );
};

export default ParkingList;
