using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ComplaintController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("SubmitCompliant")]
        public async Task<IActionResult> PostCompliant(ComplaintDto complaintDto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Emp_Id == complaintDto.Emp_Id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            var branch = await _context.Branches.FirstOrDefaultAsync(d => d.Id == complaintDto.BranchId);
            if (branch == null)
            {
                return NotFound("Branch not found");
            }
            var position = await _context.Positions.FirstOrDefaultAsync(d => d.Id == complaintDto.PositionId);
            if (position == null)
            {
                return NotFound("Position not found");
            }
            var compliant = new Compliant
            {
                Name = complaintDto.Name,
                EmployeeId = employee.Id,
                PositionId = position.Id,
                BranchId = branch.Id,
                Remedy = complaintDto.Remedy,
                Incident = complaintDto.Incident,
                DateOfEvent = complaintDto.DateOfEvent,
                Status = "Pending",
            };
            _context.Compliants.Add(compliant);
            await _context.SaveChangesAsync();

            return Ok("Compliant submitted successfully");
        }
        [HttpPost]
        [Route("AddressCompliant/{complaintId}")]
        public async Task<IActionResult> AddressCompliant(int complaintId)
        {
            var complaint = await _context.Compliants.FindAsync(complaintId);
            if (complaint == null)
            {
                return NotFound("Complaint not found");
            }

            complaint.Status = "Addressed";

            await _context.SaveChangesAsync();

            return Ok();

        }
        [HttpGet]
        [Route("MyCompliants")]
        public async Task<IActionResult> MyCompliants([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in claims");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var compliantRequests = await _context.Compliants
                .Where(c => c.EmployeeId == user.EmployeeId)
                .Select(c => new
                {
                    c.Id,
                    EmployeeId = c.Employee.Emp_Id,
                    EmployeeName = c.Employee.FirstName,
                    DateOfEvent = c.DateOfEvent,
                    Incident = c.Incident,
                    Remedy = c.Remedy,
                    SubmittedDate = c.SubmittedDate,
                    Status = c.Status
                })
                .ToListAsync();

            return Ok(compliantRequests);
        }
        [HttpGet]
        [Route("AllComplaints")]
        public async Task<IActionResult> GetAllComplaints()
        {
            var compliantRequests = await _context.Compliants
                .Select(c => new
                {
                    c.Id,
                    EmployeeId = c.Employee.Emp_Id,
                    EmployeeName = c.Employee.FirstName,
                    DateOfEvent = c.DateOfEvent,
                    Incident = c.Incident,
                    Remedy = c.Remedy,
                    SubmittedDate = c.SubmittedDate,
                    Status = c.Status
                })
                .ToListAsync();

            return Ok(compliantRequests);
        }
        [HttpGet]
        [Route("PendingComplaints")]
        public async Task<IActionResult> GetPendingComplaints()
        {
            var compliantRequests = await _context.Compliants
                .Where(c => c.Status == "Pending")
                .Select(c => new
                {
                    c.Id,
                    EmployeeId = c.Employee.Emp_Id,
                    EmployeeName = c.Employee.FirstName,
                    DateOfEvent = c.DateOfEvent,
                    Incident = c.Incident,
                    Remedy = c.Remedy,
                    SubmittedDate = c.SubmittedDate,
                    Status = c.Status
                })
                .ToListAsync();

            return Ok(compliantRequests);
        }

        [HttpGet]
        [Route("AddressedComplaints")]
        public async Task<IActionResult> GetAddressedComplaints()
        {
            var compliantRequests = await _context.Compliants
                .Where(c => c.Status == "Addressed")
                .Select(c => new
                {
                    c.Id,
                    EmployeeId = c.Employee.Emp_Id,
                    EmployeeName = c.Employee.FirstName,
                    DateOfEvent = c.DateOfEvent,
                    Incident = c.Incident,
                    Remedy = c.Remedy,
                    SubmittedDate = c.SubmittedDate,
                    Status = c.Status
                })
                .ToListAsync();

            return Ok(compliantRequests);
        }
        [HttpGet]
        [Route("PendingComplaintCount")]
        public async Task<IActionResult> GetPendingComplaintCount()
        {
            var pendingCount = await _context.Compliants
                .Where(c => c.Status == "Pending")
                .CountAsync();

            return Ok(pendingCount);
        }

        [HttpGet]
        [Route("AddressedComplaintCount")]
        public async Task<IActionResult> GetAddressedComplaintCount()
        {
            var addressedCount = await _context.Compliants
                .Where(c => c.Status == "Addressed")
                .CountAsync();

            return Ok(addressedCount);
        }


    }
}
