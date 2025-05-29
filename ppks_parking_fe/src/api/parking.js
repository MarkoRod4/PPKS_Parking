const API_BASE = process.env.REACT_APP_API_BASE_URL;

export async function fetchParkings() {
  const response = await fetch(`${API_BASE}/api/parking`);
  if (!response.ok) {
    throw new Error("Neuspješno dohvaćanje parking podataka");
  }
  return await response.json();
}