using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPKS_Parking_BE_API.Data;
using PPKS_Parking_BE_API.Models;

namespace PPKS_Parking_BE_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parking>>> GetAll()
        {
            try
            {
                var parkings = await _context.Parkings
                    .Include(p => p.Blocks)
                        .ThenInclude(b => b.ParkingSpots)
                    .ToListAsync();

                return Ok(parkings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greska prilikom dohvata parkinga: " + ex.Message);
                return StatusCode(500, "Greška na serveru");
            }
        }

    }
}

