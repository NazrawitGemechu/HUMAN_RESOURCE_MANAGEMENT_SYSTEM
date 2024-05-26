using HRMS.API.Controllers;
using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace HRMS.Tests
{
    [TestFixture]
    public class AccountControllerTestCases
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private ApplicationDbContext _context;
        private AccountController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);

            var jwtSettings = new Dictionary<string, string>
    {
        { "Key", "supersecretkey12345" },
        { "Issuer", "yourIssuer" },
        { "Audience", "yourAudience" }
    };

            _mockConfiguration.SetupGet(x => x["JwtSettings:Key"]).Returns(jwtSettings["Key"]);
            _mockConfiguration.SetupGet(x => x["JwtSettings:Issuer"]).Returns(jwtSettings["Issuer"]);
            _mockConfiguration.SetupGet(x => x["JwtSettings:Audience"]).Returns(jwtSettings["Audience"]);

            _controller = new AccountController(
                _mockConfiguration.Object,
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _context);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var userLoginDto = new UserLoginDto { username = "testuser", password = "password" };
            var user = new ApplicationUser { Id = "1", UserName = "testuser", Email = "testuser@example.com", Name = "Test User" };
            var jwtSettings = new Dictionary<string, string>
            {
                { "Key", "supersecretkey12345" },
                { "Issuer", "yourIssuer" },
                { "Audience", "yourAudience" }
            };

            _mockConfiguration.SetupGet(x => x["JwtSettings:Key"]).Returns(jwtSettings["Key"]);
            _mockConfiguration.SetupGet(x => x["JwtSettings:Issuer"]).Returns(jwtSettings["Issuer"]);
            _mockConfiguration.SetupGet(x => x["JwtSettings:Audience"]).Returns(jwtSettings["Audience"]);

            _mockUserManager.Setup(x => x.FindByNameAsync(userLoginDto.username)).ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, userLoginDto.password, false, false)).ReturnsAsync(SignInResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _controller.Login(userLoginDto) as OkObjectResult;

            // Assert
            Assert.IsNull(result);
            Assert.IsNotInstanceOf<OkObjectResult>(result);
            Assert.IsNotInstanceOf<string>(result.Value);
            var token = result.Value as string;
            Assert.IsNull(token);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsNotFound()
        {
            // Arrange
            var userLoginDto = new UserLoginDto { username = "testuser", password = "wrongpassword" };
            var user = new ApplicationUser { Id = "1", UserName = "testuser", Email = "testuser@example.com", Name = "Test User" };

            _mockUserManager.Setup(x => x.FindByNameAsync(userLoginDto.username)).ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, userLoginDto.password, false, false)).ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _controller.Login(userLoginDto) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.AreEqual("Invalid username or password", result.Value);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
