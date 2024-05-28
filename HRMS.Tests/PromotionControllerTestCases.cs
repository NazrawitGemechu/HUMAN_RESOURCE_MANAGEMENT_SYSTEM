using HRMS.API.Controllers;
using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Tests
{
    [TestFixture]
    public class PromotionControllerTestCases
    {
        private ApplicationDbContext _context;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private PromotionController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var employee = new Employee
            {
                Id = 1,
                Emp_Id = "EMP001",
                FirstName = "Nazrawit",
                LastName = "Gemechu",
                MotherName = "Saron",
                Email = "nazrawitgemechu@gmail.com",
                DepartmentId = 1,
                GradeId = 1,
                PositionId = 1,
                BranchId = 1,
                DegreeId = 1,
                HireDate = DateTime.Now,
                Salary = 50000,
                Roles = "Admin",
                Gender = "Female",
                MaritalStatus = "Single",
                Woreda = "Test Woreda",
                Kebele = 123,
                HouseNo = "123",
                Region = "Test Region",
                PhoneNo = "0935253622"
            };

            var user = new ApplicationUser
            {
                Id = "user1",
                Name = "Nazrawit",
                Email = "nazrawitgemechu@gmail.com",
                EmployeeId = 1
            };

            _context.Employees.Add(employee);
            _context.Users.Add(user);
            _context.SaveChanges();

            _controller = new PromotionController(_context, _mockUserManager.Object);
        }

        [Test]
        public async Task PostJob_ReturnsOk_WhenJobIsPostedSuccessfully()
        {
            var jobDto = new InternalJobDto
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };

            var actionResult = await _controller.PostJob(jobDto);

            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Job posted successfully", okResult.Value);
        }

        [Test]
        public async Task ApplyForJob_ReturnsOk_WhenJobApplicationIsSuccessful()
        {
            var job = new InternalJob
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            var actionResult = await _controller.ApplyForJob(job.Id, "user1");

            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Job application submitted successfully", okResult.Value);

            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.EmployeeId == 1);
            Assert.IsNotNull(notification);
            Assert.AreEqual("Job Application", notification.Title);
            Assert.AreEqual("Your have successfully applied for the job. We will notify you on our decision.", notification.Message);
        }

        [Test]
        public async Task ApplyForJob_ReturnsUnauthorized_WhenUserIdIsMissing()
        {
            var job = new InternalJob
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            var actionResult = await _controller.ApplyForJob(job.Id, null);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(actionResult);
            var unauthorizedResult = actionResult as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("User ID not found in claims", unauthorizedResult.Value);
        }

        [Test]
        public async Task ApplyForJob_ReturnsNotFound_WhenUserNotFound()
        {
            var job = new InternalJob
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            var actionResult = await _controller.ApplyForJob(job.Id, "nonexistentUser");

            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("User not found", notFoundResult.Value);
        }

        [Test]
        public async Task ApplyForJob_ReturnsNotFound_WhenEmployeeNotFound()
        {
            var job = new InternalJob
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            var user = new ApplicationUser
            {
                Id = "user2",
                Name = "nonexistent user",
                Email = "nonexistent@gmail.com",
                EmployeeId = 999 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var actionResult = await _controller.ApplyForJob(job.Id, "user2");

            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Employee not found", notFoundResult.Value);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
