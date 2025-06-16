namespace PPKS_Parking_BE_API.Models
{
    public class ParkingSpotUsageLog
    {
        public int Id { get; set; }

        public int ParkingSpotId { get; set; }
        public ParkingSpot ParkingSpot { get; set; }

        public bool IsOccupied { get; set; }

        public DateTime Timestamp { get; set; }
    }

}
