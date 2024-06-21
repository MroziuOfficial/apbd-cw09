using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apbd_cw09.Data;
using apbd_cw09.Models;

namespace apbd_cw09.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly Apbd09Context _context;

        public TripsController(Apbd09Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tripsQuery = _context.Trips
                .Include(t => t.IdCountries)
                .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
                .OrderByDescending(t => t.DateFrom);

            var totalTrips = await tripsQuery.CountAsync();
            var trips = await tripsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new
            {
                pageNum = page,
                pageSize = pageSize,
                allPages = (int)Math.Ceiling(totalTrips / (double)pageSize),
                trips = trips.Select(t => new
                {
                    t.Name,
                    t.Description,
                    t.DateFrom,
                    t.DateTo,
                    t.MaxPeople,
                    Countries = t.IdCountries.Select(c => new { c.Name }),
                    Clients = t.ClientTrips.Select(ct => new
                    {
                        ct.IdClientNavigation.FirstName,
                        ct.IdClientNavigation.LastName
                    })
                })
            };

            return Ok(result);
        }
    }
}