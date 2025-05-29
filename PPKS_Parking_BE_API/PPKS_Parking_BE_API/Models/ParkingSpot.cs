namespace PPKS_Parking_BE_API.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }

        public int BlockId { get; set; }

        public virtual Block Block { get; set; }
        public string Name { get; set; }

        public bool IsOccupied { get; set; }
    }
}
