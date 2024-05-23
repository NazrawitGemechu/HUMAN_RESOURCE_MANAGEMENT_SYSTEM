using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DegreeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DegreeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Degree>>> GetDegrees()
        {
            return await _context.Degrees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DegreeDto>> GetDegree(int id)
        {
            var degree = await _context.Degrees.FindAsync(id);

            if (degree == null)
            {
                return NotFound();
            }

            var degreeDto = new DegreeDto
            {
                Name = degree.Name
            };

            return degreeDto;
        }

        [HttpPost]
        public async Task<ActionResult<DegreeDto>> PostDegree(DegreeDto degreeDto)
        {
            var degree = new Degree
            {
                Name = degreeDto.Name
            };

            _context.Degrees.Add(degree);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDegree), new { id = degree.Id }, degreeDto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDegree(int id, DegreeDto degreeDto)
        {
            var degree = await _context.Degrees.FindAsync(id);
            if (degree == null)
            {
                return NotFound();
            }

            degree.Name = degreeDto.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DegreeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDegree(int id)
        {
            var degree = await _context.Degrees.FindAsync(id);
            if (degree == null)
            {
                return NotFound();
            }

            _context.Degrees.Remove(degree);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool DegreeExists(int id)
        {
            return _context.Degrees.Any(e => e.Id == id);
        }
    }
}
