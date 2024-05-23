using HRMS.API.Data;
using HRMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRMS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        [Route("GetNotificationsForEmployee")]
        public async Task<IEnumerable<Notification>> GetNotificationsForEmployee()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var employeeId = user.EmployeeId;

            // Retrieve notifications for the employee ID
            return await _context.Notifications
                .Where(n => n.EmployeeId == employeeId)
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();
        }
        [HttpGet]
        [Route("GetUnreadNotificationCount")]
        public async Task<IActionResult> GetUnreadNotificationCount([FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var employeeId = user.EmployeeId;
            if (employeeId == null)
            {
                return NotFound("Employee not found for the given user ID");
            }
            var unreadCount = await _context.Notifications
                .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                .CountAsync();
            return Ok(unreadCount);

        }
        [HttpGet]
        [Route("GetReadNotificationCount")]
        public async Task<IActionResult> GetReadNotificationCount([FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var employeeId = user.EmployeeId;
            if (employeeId == null)
            {
                return NotFound("Employee not found for the given user ID");
            }
            var readCount = await _context.Notifications
                .Where(n => n.EmployeeId == employeeId && n.IsRead)
                .CountAsync();
            return Ok(readCount);
        }
        [HttpPatch]
        [Route("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var read = await _context.Notifications.FindAsync(id);
            if (read == null)
            {
                return NotFound("Leave not found");
            }

            read.IsRead = true;

            await _context.SaveChangesAsync();
            return Ok("Marked as read");
        }
        [HttpGet]
        [Route("GetUnreadNotificationsList")]
        public async Task<IActionResult> GetUnreadNotifications([FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var employeeId = user.EmployeeId;
            if (employeeId == null)
            {
                return NotFound("Employee not found for the given user ID");
            }

            var unreadNotifications = await _context.Notifications
                .Where(n => n.EmployeeId == employeeId && n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
            {
                return Ok(new List<Notification>());
            }

            return Ok(unreadNotifications);
        }

    }
}
