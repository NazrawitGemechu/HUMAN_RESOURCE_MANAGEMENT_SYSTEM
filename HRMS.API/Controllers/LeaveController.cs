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
    public class LeaveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public LeaveController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet("ListOfLeaveRequests")]
        public async Task<IActionResult> GetLeaveRequests()
        {
            var leaveRequests = new List<object>();

            var leaves = await _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .ToListAsync();

            foreach (var leave in leaves)
            {
                var remainingLeaveBalance = await GetRemainingLeaveBalance(leave.EmployeeId, leave.LeaveTypeId);
                var leaveRequest = new
                {
                    leave.Id,
                    EmployeeId = leave.Employee.Emp_Id,
                    EmployeeName = leave.Employee.FirstName,
                    LeaveTypeName = leave.LeaveType.Name,
                    AllowedDays = leave.LeaveType.AllowedDays,
                    RemainingLeaveBalance = remainingLeaveBalance,
                    leave.StartDate,
                    leave.EndDate,
                    Status = leave.Status,
                };

                leaveRequests.Add(leaveRequest);
            }

            return Ok(leaveRequests);
        }

        [HttpGet]
        [Route("MyLeaveRequests")]
        public async Task<IActionResult> MyLeaveRequests([FromQuery] string userId)
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

            var leaveRequests = new List<object>();

            var leaves = await _context.Leaves
                        .Include(l => l.Employee)
                        .Include(l => l.LeaveType)
                        .Join(_context.Users,
                              leave => leave.EmployeeId,
                              user => user.EmployeeId,
                              (leave, user) => new { Leave = leave, User = user })
                        .Where(l => l.User.Id == userId)
                        .Select(l => l.Leave)
                        .ToListAsync();



            foreach (var leave in leaves)
            {
                var remainingLeaveBalance = await GetRemainingLeaveBalance(leave.EmployeeId, leave.LeaveTypeId);
                var leaveRequest = new
                {
                    leave.Id,
                    EmployeeId = leave.Employee.Emp_Id,
                    EmployeeName = leave.Employee.FirstName,
                    LeaveTypeName = leave.LeaveType.Name,
                    AllowedDays = leave.LeaveType.AllowedDays,
                    RemainingLeaveBalance = remainingLeaveBalance,
                    leave.StartDate,
                    leave.EndDate,
                    Status = leave.Status,
                };

                leaveRequests.Add(leaveRequest);
            }

            return Ok(leaveRequests);
        }

        public async Task<int> GetRemainingLeaveBalance(int employeeId, int leaveTypeId)
        {
            var leaveBalance = await _context.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.LeaveTypeId == leaveTypeId);

            return leaveBalance?.NumberOfDays ?? 0;
        }

        [HttpPost]
        [Route("RequestLeave")]
        public async Task<IActionResult> RequestLeave(LeaveRequestDto leaveRequestDto)
        {

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Emp_Id == leaveRequestDto.Emp_Id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var remainingLeaveBalance = await CalculateRemainingLeaveBalance(employee.Id, leaveRequestDto.LeaveTypeId);
            var requestedLeaveDays = (leaveRequestDto.EndDate - leaveRequestDto.StartDate).Days + 1;

            if (requestedLeaveDays > remainingLeaveBalance)
            {
                return BadRequest("Insufficient leave balance");
            }

            var leave = new Leave
            {
                EmployeeId = employee.Id,
                LeaveTypeId = leaveRequestDto.LeaveTypeId,
                StartDate = leaveRequestDto.StartDate,
                EndDate = leaveRequestDto.EndDate,
                Reason = leaveRequestDto.Reason,
                Status = "Pending",

            };

            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();

            return Ok("Leave request submitted successfully");
        }
        [HttpPost]
        [Route("ApproveLeave/{leaveId}")]
        public async Task<IActionResult> ApproveLeave(int leaveId)
        {
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound("Leave not found");
            }

            leave.Status = "Approved";

            await _context.SaveChangesAsync();

            var remainingBalance = await CalculateRemainingLeaveBalance(leave.EmployeeId, leave.LeaveTypeId);
            await UpdateLeaveBalance(leave.EmployeeId, leave.LeaveTypeId, remainingBalance);

            var notification = new Notification
            {
                Title = "Leave Request",
                Message = $"Congratulations! Your leave request have been approved.",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = leave.EmployeeId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok("Leave request approved successfully");
        }
        [HttpPost]
        [Route("RejectLeave/{leaveId}")]
        public async Task<IActionResult> RejectLeave(int leaveId)
        {
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound("Leave not found");
            }
            leave.Status = "Rejected";
            await _context.SaveChangesAsync();


            var notification = new Notification
            {
                Title = "Leave Request",
                Message = $"Your leave request have been rejected.Contact HR for more information",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = leave.EmployeeId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return Ok("Leave request rejected successfully");
        }
        [HttpPost]
        [Route("LeaveBalance")]
        public async Task<int> CalculateRemainingLeaveBalance(int employeeId, int leaveTypeId)
        {

            var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
            if (leaveType == null)
            {
                throw new ArgumentException("Invalid leave type");
            }
            var totalLeaveDaysTaken = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId && l.LeaveTypeId == leaveTypeId && l.Status == "Approved")
                .SumAsync(l => (EF.Functions.DateDiffDay(l.StartDate, l.EndDate) + 1));

            var leaveBalance = await _context.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.LeaveTypeId == leaveTypeId);
            int remainingLeaveBalance;
            if (leaveBalance != null)
            {
                remainingLeaveBalance = leaveBalance.NumberOfDays;
            }
            else
            {
                remainingLeaveBalance = leaveType.AllowedDays;
            }
            remainingLeaveBalance -= totalLeaveDaysTaken;

            return remainingLeaveBalance;
        }
        [HttpPost]
        [Route("Update Leave Balance ")]
        private async Task UpdateLeaveBalance(int employeeId, int leaveTypeId, int remainingBalance)
        {
            var leaveBalance = await _context.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.LeaveTypeId == leaveTypeId);

            if (leaveBalance != null)
            {
                leaveBalance.NumberOfDays = remainingBalance;
            }
            else
            {
                leaveBalance = new LeaveBalance
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveTypeId,
                    NumberOfDays = remainingBalance
                };
                _context.LeaveBalances.Add(leaveBalance);
            }
            await _context.SaveChangesAsync();
        }
        [HttpGet]
        [Route("GetPendingLeaveRequests")]
        public async Task<IActionResult> GetPendingLeaveRequests()
        {
            var pendingLeaveRequests = _context.Leaves.Where(l => l.Status == "Pending");
            return Ok(pendingLeaveRequests);
        }
        [HttpGet]
        [Route("GetApprovedLeaveRequests")]
        public async Task<IActionResult> GetApprovedLeaveRequests()
        {
            Leave leave = new Leave();
            var approvedLeaveRequests = _context.Leaves.Where(l => l.Status == "Approved");
            return Ok(approvedLeaveRequests);
        }
        [HttpGet]
        [Route("GetRejectedLeaveRequests")]
        public async Task<IActionResult> GetRejectedLeaveRequests()
        {
            Leave leave = new Leave();
            var rejectedLeaveRequests = _context.Leaves.Where(l => l.Status == "Rejected");
            return Ok(rejectedLeaveRequests);
        }

        [HttpGet]
        [Route("GetMyLeaveBalance")]
        public async Task<IActionResult> MyLeaveBalance([FromQuery] string userId)
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

            var employeeId = user.EmployeeId;
            if (employeeId == null)
            {
                return NotFound("Employee ID not found for the user");
            }

            var leaveBalances = new List<object>();

            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            var employeeLeaveBalances = await _context.LeaveBalances
                .Include(lb => lb.LeaveType)
                .Where(lb => lb.EmployeeId == employeeId)
                .ToListAsync();

            foreach (var leaveType in leaveTypes)
            {
                var leaveBalance = employeeLeaveBalances.FirstOrDefault(lb => lb.LeaveTypeId == leaveType.Id);

                var leaveBalanceInfo = new
                {
                    LeaveTypeName = leaveType.Name,
                    AllowedDays = leaveType.AllowedDays,
                    RemainingLeaveBalance = leaveBalance != null ? leaveBalance.NumberOfDays : leaveType.AllowedDays
                };

                leaveBalances.Add(leaveBalanceInfo);
            }

            return Ok(leaveBalances);
        }



    }
}
