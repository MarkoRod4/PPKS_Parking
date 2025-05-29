namespace PPKS_Parking_BE_API.Models
{
    public class Block
    {
        public int Id { get; set; }
        public int ParkingId { get; set; }
        public virtual Parking Parking { get; set; }
        public string Name { get; set; }
        public ICollection<ParkingSpot> ParkingSpots { get; set; }
    }
}
