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
    }
}
