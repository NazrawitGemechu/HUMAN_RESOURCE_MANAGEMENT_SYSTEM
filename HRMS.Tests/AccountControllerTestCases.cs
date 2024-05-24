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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace HRMS.Tests
{
    [TestFixture]
    public class AccountControllerTestCases
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            // Example setup for SignInManager mock with more constructor arguments
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                _httpContextAccessorMock.Object,
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null
            );

            _configurationMock = new Mock<IConfiguration>();
            _context = CreateInMemoryContext();
            SeedData();
        }

        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "HRManagement")
                .Options;
            return new ApplicationDbContext(options);
        }

        private void SeedData()
        {
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" }
            };

            var grades = new List<Grade>
            {
                new Grade { Id = 1, GradeName = "A" },
                new Grade { Id = 2, GradeName = "B" }
            };

            var positions = new List<Position>
            {
                new Position { Id = 1, Name = "Developer" },
                new Position { Id = 2, Name = "Manager" }
            };

            var branches = new List<Branch>
            {
                new Branch { Id = 1, Name = "Branch1" },
                new Branch { Id = 2, Name = "Branch2" }
            };

            var degrees = new List<Degree>
            {
                new Degree { Id = 1, Name = "BSc" },
                new Degree { Id = 2, Name = "MSc" }
            };

            _context.Departments.AddRange(departments);
            _context.Grades.AddRange(grades);
            _context.Positions.AddRange(positions);
            _context.Branches.AddRange(branches);
            _context.Degrees.AddRange(degrees);

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Emp_Id = "EMP001",
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = "Male",
                    MotherName = "Jane Doe",
                    PhoneNo = "1234567890",
                    Roles = "Developer",
                    Email = "john.doe@example.com",
                    MaritalStatus = "Single",
                    Region = "Region1",
                    Woreda = "Woreda1",
                    Kebele = 1,
                    HouseNo = "House1",
                    DepartmentId = 1,
                    GradeId = 1,
                    PositionId = 1,
                    BranchId = 1,
                    DegreeId = 1,
                    HireDate = new DateTime(2020, 1, 1),
                    Salary = 50000,
                    ContactPersons = new List<ContactPerson>
                    {
                        new ContactPerson { Id = 1, Name = "Emergency Contact 1", Relationship = "Friend", PhoneNo = "1111111111", Region = "Region1", Woreda = "Woreda1", Kebele = 1, HouseNo = "House1" }
                    },
                    Educations = new List<Education>
                    {
                        new Education { Id = 1, Degree = "BSc", Institute = "Institute1" }
                    },
                    Experiences = new List<Experience>
                    {
                        new Experience { Id = 1, CompanyName = "Company1", Position = "Junior Developer", StartDate = new DateTime(2018, 1, 1), EndDate = new DateTime(2020, 1, 1) }
                    }
                }
            };

            _context.Employees.AddRange(employees);
            _context.SaveChanges();
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new ApplicationUser { Name = "testuser", EmployeeId=1 };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(user));
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Success));

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("supersecretkey123");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");

            _configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSectionMock.Object);

            var controller = new AccountController(_configurationMock.Object, _userManagerMock.Object, _signInManagerMock.Object, _context);

            var loginDto = new UserLoginDto { username = "testuser", password = "password" };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult.Value); // Token should be present
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsNotFound()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult((ApplicationUser)null));
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Failed));

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("supersecretkey123");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");

            _configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSectionMock.Object);

            var controller = new AccountController(_configurationMock.Object, _userManagerMock.Object, _signInManagerMock.Object, _context);

            var loginDto = new UserLoginDto { username = "invaliduser", password = "wrongpassword" };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Invalid username or password", notFoundResult.Value);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
