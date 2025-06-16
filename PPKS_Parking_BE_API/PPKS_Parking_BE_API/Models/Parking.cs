using System.ComponentModel.DataAnnotations.Schema;

namespace PPKS_Parking_BE_API.Models
{
    public class Parking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }

        [NotMapped]
        public int FreeSpotsCount => Blocks?.SelectMany(b => b.ParkingSpots).Count(ps => !ps.IsOccupied) ?? 0;
        [NotMapped]

        public double Occupancy
        {
            get
            {
                var spots = Blocks?.SelectMany(b => b.ParkingSpots).ToList();
                if (spots == null || spots.Count == 0)
                    return 0;

                var occupiedCount = spots.Count(ps => ps.IsOccupied);
                return (double)occupiedCount / spots.Count * 100;
            }
        }
    }
}
