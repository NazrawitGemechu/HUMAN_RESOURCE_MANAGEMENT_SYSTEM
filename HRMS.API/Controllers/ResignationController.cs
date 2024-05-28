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
    public class ResignationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ResignationController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("ListOfResignationRequests")]
        public async Task<IActionResult> GetResignationRequests()
        {
            var resignationRequests = new List<object>();

            var resignations = await _context.Resignations
                .Include(l => l.Employee)
                .Include(r => r.Position)
                .Include(r => r.Department)
                .ToListAsync();

            foreach (var resignation in resignations)
            {
                var resignationRequest = new
                {

                    resignation.Id,
                    EmployeeId = resignation.Employee.Emp_Id,
                    FullName = resignation.Employee.FirstName,
                    PositionId = resignation.Position.Id,
                    DepartmentId = resignation.Department.Id,
                    resignation.EmployeeHireDate,
                    resignation.SeparationDate,
                    resignation.Reason,
                    resignation.Satisfaction,
                    resignation.EmployeeRelationship,
                    resignation.Recommendation,
                    resignation.Comment,
                    Status = resignation.Status,
                };

                resignationRequests.Add(resignationRequest);
            }

            return Ok(resignationRequests);
        }

        [HttpPost]
        [Route("RequestResignation")]
        public async Task<IActionResult> RequestLeave(ResignationDto resignationDto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Emp_Id == resignationDto.Emp_Id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == employee.DepartmentId);
            if (department == null)
            {
                return NotFound("Department not found");
            }
            var position = await _context.Positions.FirstOrDefaultAsync(d => d.Id == employee.PositionId);
            if (position == null)
            {
                return NotFound("Position not found");
            }
            var resignation = new Resignation
            {

                EmployeeId = employee.Id,
                FullName = resignationDto.FullName,
                DepartmentId = resignationDto.DepartmentId,
                PositionId = resignationDto.PositionId,
                SeparationDate = resignationDto.SeparationDate,
                Reason = resignationDto.Reason,
                Satisfaction = resignationDto.Satisfaction,
                EmployeeRelationship = resignationDto.EmployeeRelationship,
                Recommendation = resignationDto.Recommendation,
                Comment = resignationDto.Comment,
                EmployeeHireDate = resignationDto.EmployeeHireDate,
                Status = "Pending",

            };
            _context.Resignations.Add(resignation);
            await _context.SaveChangesAsync();

            return Ok("Resignation request submitted successfully");
        }

        [HttpPost]
        [Route("ApproveResignation/{resignationId}")]
        public async Task<IActionResult> ApproveResignation(int resignationId)
        {
            var resignation = await _context.Resignations.FindAsync(resignationId);
            if (resignation == null)
            {
                return NotFound("Resignation not found");
            }

            resignation.Status = "Approved";

            var employee = await _context.Employees.FindAsync(resignation.EmployeeId);
            if (employee != null)
            {
                employee.Status = "Inactive";
            }
            await _context.SaveChangesAsync();


            var notification = new Notification
            {
                Title = "Resignation Request",
                Message = $"Congratulations! Your resignation request have been accepted.",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = resignation.EmployeeId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return Ok("Resignation request approved successfully");
        }

        [HttpPost]
        [Route("RejectResignation/{resignationId}")]
        public async Task<IActionResult> RejectResignation(int resignationId)
        {
            var resignation = await _context.Resignations.FindAsync(resignationId);
            if (resignation == null)
            {
                return NotFound("Resignation not found");
            }

            resignation.Status = "Rejected";

            await _context.SaveChangesAsync();
            var notification = new Notification
            {
                Title = "Resignation Request",
                Message = $"Your resignation request has not been accepted.Please contact HR for more information.",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = resignation.EmployeeId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok("Resignation request rejected successfully");
        }
        [HttpGet]
        [Route("CountPendingResignations")]
        public async Task<IActionResult> CountPendingResignations()
        {
            var pendingCount = await _context.Resignations.CountAsync(r => r.Status == "Pending");
            return Ok(pendingCount);
        }
        [HttpGet]
        [Route("CountApprovedResignations")]
        public async Task<IActionResult> CountApprovedResignations()
        {
            var approvedCount = await _context.Resignations.CountAsync(r => r.Status == "Approved");
            return Ok(approvedCount);
        }

    }
}
