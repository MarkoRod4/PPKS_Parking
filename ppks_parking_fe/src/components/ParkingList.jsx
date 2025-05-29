import React from "react";

const ParkingList = ({ parkings }) => {
  return (
    <div className="grid gap-4">
      {parkings.map(p => (
        <div key={p.id} className="border p-4 rounded shadow">
          <h2 className="text-xl font-bold">{p.name}</h2>
          <p>Slobodna mjesta: {p.freeSpotsCount}</p>
        </div>
      ))}
    </div>
  );
};

export default ParkingList;
