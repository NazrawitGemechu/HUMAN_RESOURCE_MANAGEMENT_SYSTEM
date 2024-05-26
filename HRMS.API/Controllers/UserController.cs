using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var userList = _context.ApplicationUsers.Include(u => u.Employee).ToList();
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            var userDtoList = new List<UserDto>();

            foreach (var user in userList)
            {
                var userDto = new UserDto
                {
                    Name = user.Name,
                    PictureURL = user.pictureURL
                };

                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    userDto.Roles = user.Employee != null ? "None" : null;
                }
                else
                {
                    userDto.Roles = user.Employee != null ? roles.FirstOrDefault(u => u.Id == role.RoleId)?.Name : null;
                }

                userDtoList.Add(userDto);
            }

            return Ok(userDtoList);
        }
        [HttpPost]
        [Route("LockOrUnlockUser")]
        public IActionResult LockUnlock([FromQuery] string userId)
        {
            var objFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is locked and will remain locked out untill lockout item
                //clicking on this action will unlock them
                objFromDb.LockoutEnd = DateTime.Now;

            }
            else
            {
                //user is not locked, and we want to lock the user
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100);


            }
            _context.SaveChanges();
            return Ok();


        }

        [HttpPost]
        [Route("Delete User")]
        public IActionResult Delete(string userId)
        {
            var objFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            _context.ApplicationUsers.Remove(objFromDb);
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("UsersCount")]
        public async Task<IActionResult> GetUsersCount()
        {
            try
            {
                var usersCount = await _context.ApplicationUsers
                    .CountAsync();
                return Ok(usersCount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving users count: {ex.Message}");
            }
        }
    }
}
