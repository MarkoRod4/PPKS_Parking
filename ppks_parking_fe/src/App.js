import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Index from "./pages/Index";
import ParkingDetails from "./pages/ParkingDetails";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Index />} />
        <Route path="/parking/:id" element={<ParkingDetails />} />
      </Routes>
    </Router>
  );
}

export default App;
