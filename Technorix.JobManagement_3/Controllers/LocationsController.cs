using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using Technorix.JobManagement_3.Model;

namespace Technorix.JobManagement_3.Controllers
{
   
    [Route("api/v1/locations")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly JobDbContext _context;

        public LocationsController(JobDbContext context)
        {
            _context = context;
        }

        // POST /api/v1/locations
        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return Created($"/api/v1/locations/{location.Id}", location);
        }

        // PUT /api/v1/locations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location location)
        {
            var existingLocation = await _context.Locations.FindAsync(id);
            if (existingLocation == null) return NotFound();

            existingLocation.Title = location.Title;
            existingLocation.City = location.City;
            existingLocation.State = location.State;
            existingLocation.Country = location.Country;
            existingLocation.Zip = location.Zip;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET /api/v1/locations
        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();
            return Ok(locations);
        }
    }
}
