using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LeaveTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveTypeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetLeaveTypes")]
        public async Task<ActionResult<IEnumerable<LeaveType>>> GetLeaveTypes()
        {
            return await _context.LeaveTypes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveType>> GetLeaveType(int id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);

            if (leaveType == null)
            {
                return NotFound();
            }

            return leaveType;
        }

        [HttpPost]
        [Route("AddLeaveTypes")]
        public async Task<ActionResult<LeaveType>> PostLeaveType(LeaveTypeDto leaveTypeDto)
        {
            LeaveType leaveType = new LeaveType()
            {
                Name = leaveTypeDto.Name,
                AllowedDays = leaveTypeDto.AllowedDays,
            };
            _context.LeaveTypes.Add(leaveType);
            await _context.SaveChangesAsync();

            return Ok("Added Succesfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveType(int id, LeaveTypeDetailDto leaveTypeDetailDto)
        {


            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            leaveType.Name = leaveTypeDetailDto.Name;
            leaveType.AllowedDays = leaveTypeDetailDto.AllowedDays;
            _context.Entry(leaveType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveTypeExists(id))
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
        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }

            _context.LeaveTypes.Remove(leaveType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveTypeExists(int id)
        {
            return _context.LeaveTypes.Any(e => e.Id == id);
        }
    }
}
