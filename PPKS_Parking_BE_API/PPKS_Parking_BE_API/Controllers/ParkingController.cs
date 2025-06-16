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

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var parking = await _context.Parkings
                    .Include(p => p.Blocks)
                        .ThenInclude(b => b.ParkingSpots)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (parking == null)
                    return NotFound("Parking nije pronađen.");

                var allSpots = parking.Blocks.SelectMany(b => b.ParkingSpots).ToList();

                var totalSpots = allSpots.Count;
                var freeSpots = parking.FreeSpotsCount;
                var occupiedSpots = totalSpots - freeSpots;

                var spotIds = allSpots.Select(s => s.Id).ToList();

                var logs = await _context.ParkingSpotUsageLogs
                    .Where(log => spotIds.Contains(log.ParkingSpotId))
                    .ToListAsync();

                var weeklyStats = logs
                    .GroupBy(l => l.Timestamp.DayOfWeek)
                    .Select(g => new {
                        Day = g.Key.ToString(),
                        TotalRecords = g.Count(),
                        AvgOccupiedPercent = g.Count(x => x.IsOccupied) * 100 / g.Count()
                    })
                    .ToList();

                var dailyStats = logs
                    .GroupBy(l => new { l.Timestamp.DayOfWeek, Hour = l.Timestamp.Hour })
                    .Select(g => new {
                        Day = g.Key.DayOfWeek.ToString(),
                        Hour = g.Key.Hour,
                        TotalRecords = g.Count(),
                        AvgOccupiedPercent = g.Count(x => x.IsOccupied) * 100 / g.Count()
                    })
                    .OrderBy(x => x.Day).ThenBy(x => x.Hour)
                    .ToList();

                return Ok(new
                {
                    parking.Id,
                    parking.Name,
                    TotalSpots = totalSpots,
                    OccupiedSpots = occupiedSpots,
                    FreeSpots = freeSpots,
                    WeeklyStats = weeklyStats.Select(w => new {
                        w.Day,
                        w.TotalRecords,
                        w.AvgOccupiedPercent
                    }),
                    DailyStats = dailyStats.Select(d => new {
                        d.Day,
                        d.Hour,
                        d.TotalRecords,
                        d.AvgOccupiedPercent
                    })
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greška: " + ex.Message);
                return StatusCode(500, "Greška na serveru");
            }
        }




    }
}

