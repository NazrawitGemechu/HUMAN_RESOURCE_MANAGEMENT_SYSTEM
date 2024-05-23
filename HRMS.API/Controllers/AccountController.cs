using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRMS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AccountController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var user = await Authenticate(userLoginDto);
            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }
            return NotFound("Invalid username or password");
        }

        private string Generate(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create a list to hold all the claims
            var claimsList = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.GivenName, user.Name),
        new Claim(ClaimTypes.Email, user.Email)
    };

            // Retrieve roles associated with the user
            var roles = _userManager.GetRolesAsync(user).Result;

            // Add role claims if the user has roles
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claimsList.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var token = new JwtSecurityToken(
                jwtSettings["Issuer"],
                jwtSettings["Audience"],
                claimsList,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ApplicationUser> Authenticate(UserLoginDto userLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userLoginDto.username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, userLoginDto.password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return user;
                }
            }
            return null;
        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
                return Ok("Logged out successfully");
            }
            else
            {
                return BadRequest("User is not authenticated");
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model, [FromQuery] string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest("Something went wrong! try again.");
            }

            return Ok("Password changed successfully.");
        }
        [HttpGet("profile")]

        public async Task<IActionResult> GetUserProfile([FromQuery] string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            byte[] pictureData = null;
            if (!string.IsNullOrEmpty(user.pictureURL))
            {
                try
                {
                    pictureData = System.IO.File.ReadAllBytes(user.pictureURL);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading picture data: {ex.Message}");
                }
            }

            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                PictureURL = user.pictureURL,
                PictureData = pictureData
            };

            return Ok(userProfile);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserProfile(UserProfileUpdateDto model, [FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update photo only if provided
            if (model.PhotoData != null)
            {
                var fileName = Path.GetFileName(model.PhotoData.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                // Delete existing photo if present (optional, modify if needed)
                var existingPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", user.pictureURL.Split('/').Last());
                if (System.IO.File.Exists(existingPhotoPath))
                {
                    System.IO.File.Delete(existingPhotoPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PhotoData.CopyToAsync(stream);
                }

                user.pictureURL = "/images/" + fileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Profile photo updated successfully");
            }
            else
            {
                return BadRequest("Failed to update profile photo");
            }
        }

    }
}
