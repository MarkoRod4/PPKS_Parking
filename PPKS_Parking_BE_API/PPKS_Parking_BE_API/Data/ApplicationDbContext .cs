using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PPKS_Parking_BE_API.Models;

namespace PPKS_Parking_BE_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
    }
}