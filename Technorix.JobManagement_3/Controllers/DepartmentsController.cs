using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using Technorix.JobManagement_3.Model;

namespace Technorix.JobManagement_3.Controllers
{
    [Route("api/v1/departments")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly JobDbContext _context;

        public DepartmentsController(JobDbContext context)
        {
            _context = context;
        }

        // POST /api/v1/departments
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            // Validate the department input
            if (department == null || string.IsNullOrEmpty(department.Title))
            {
                return BadRequest("Department Title cannot be null or empty.");
            }

            // Add department to the context and save changes
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Return the created department with a 201 status
            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
        }

        // GET /api/v1/departments
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            // Fetch all departments
            var departments = await _context.Departments.ToListAsync();

            // Return 200 OK with the department list (empty if no departments exist)
            return Ok(departments);
        }

        // GET /api/v1/departments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            // Fetch the department by its ID
            var department = await _context.Departments.FindAsync(id);

            // If department is not found, return 404
            if (department == null)
            {
                return NotFound();
            }

            // Return 200 OK with the department
            return Ok(department);
        }

        // PUT /api/v1/departments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] Department department)
        {
            // Check if the department exists in the database
            var existingDepartment = await _context.Departments.FindAsync(id);
            if (existingDepartment == null)
            {
                return NotFound();
            }

            // Update the department title
            existingDepartment.Title = department.Title;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return 200 OK with the updated department
            return Ok(existingDepartment);
        }
    }
}
