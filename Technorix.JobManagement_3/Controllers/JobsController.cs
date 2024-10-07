using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using Technorix.JobManagement_3.Model;

namespace Technorix.JobManagement_3.Controllers
{
    [Route("api/v1/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobDbContext _context;

        public JobsController(JobDbContext context)
        {
            _context = context;
        }

        // POST /api/v1/jobs
        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] Job job)
        {
            job.Code = $"JOB-{new Random().Next(1000, 9999)}"; // Generating job code
            job.PostedDate = DateTime.UtcNow;
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return Created($"/api/v1/jobs/{job.Id}", job);
        }

        // PUT /api/v1/jobs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] Job updatedJob)
        {
            var existingJob = await _context.Jobs.FindAsync(id);
            if (existingJob == null) return NotFound();

            existingJob.Title = updatedJob.Title;
            existingJob.Description = updatedJob.Description;
            existingJob.LocationId = updatedJob.LocationId;
            existingJob.DepartmentId = updatedJob.DepartmentId;
            existingJob.ClosingDate = updatedJob.ClosingDate;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST /api/jobs/list
        [HttpPost("list")]
        public async Task<IActionResult> ListJobs([FromBody] JobSearchRequest request)
        {
            var query = _context.Jobs.Include(j => j.Location).Include(j => j.Department).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.q))
            {
                query = query.Where(j => j.Title.Contains(request.q) || j.Description.Contains(request.q));
            }

            if (request.LocationId.HasValue)
            {
                query = query.Where(j => j.LocationId == request.LocationId.Value);
            }

            if (request.DepartmentId.HasValue)
            {
                query = query.Where(j => j.DepartmentId == request.DepartmentId.Value);
            }

            var total = await query.CountAsync();

            var jobs = await query.Skip((request.PageNo - 1) * request.PageSize)
                                  .Take(request.PageSize)
                                  .Select(j => new
                                  {
                                      j.Id,
                                      Code = j.Code,
                                      j.Title,
                                      Location = j.Location.Title,
                                      Department = j.Department.Title,
                                      j.PostedDate,
                                      j.ClosingDate
                                  }).ToListAsync();

            return Ok(new { total, data = jobs });
        }

        // GET /api/v1/jobs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobDetails(int id)
        {
            var job = await _context.Jobs.Include(j => j.Location)
                                         .Include(j => j.Department)
                                         .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            return Ok(new
            {
                job.Id,
                job.Code,
                job.Title,
                job.Description,
                Location = new
                {
                    job.Location.Id,
                    job.Location.Title,
                    job.Location.City,
                    job.Location.State,
                    job.Location.Country,
                    job.Location.Zip
                },
                Department = new
                {
                    job.Department.Id,
                    job.Department.Title
                },
                job.PostedDate,
                job.ClosingDate
            });
        }
    }
}
