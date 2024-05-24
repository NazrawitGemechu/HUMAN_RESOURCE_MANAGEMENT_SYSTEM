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

            // Seed data
            var employee = new Employee
            {
                Id = 1,
                Emp_Id = "EMP001",
                FirstName = "John",
                LastName = "Doe",
                MotherName = "Mother Name",
                Email = "john.doe@example.com",
                DepartmentId = 1,
                GradeId = 1,
                PositionId = 1,
                BranchId = 1,
                DegreeId = 1,
                HireDate = DateTime.Now,
                Salary = 50000,
                Roles = "Admin",
                Gender = "Male",
                MaritalStatus = "Single",
                Woreda = "Sample Woreda",
                Kebele = 123,
                HouseNo = "123",
                Region = "Sample Region",
                PhoneNo = "0123456789"
            };

            var user = new ApplicationUser
            {
                Id = "user1",
                Name = "john.doe@example.com",
                Email = "john.doe@example.com",
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
            // Arrange
            var jobDto = new InternalJobDto
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };

            // Act
            var actionResult = await _controller.PostJob(jobDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Job posted successfully", okResult.Value);
        }

        [Test]
        public async Task ApplyForJob_ReturnsOk_WhenJobApplicationIsSuccessful()
        {
            // Arrange
            var job = new InternalJob
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            // Act
            var actionResult = await _controller.ApplyForJob(job.Id, "user1");

            // Assert
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
            // Arrange
            var job = new InternalJob
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            // Act
            var actionResult = await _controller.ApplyForJob(job.Id, null);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(actionResult);
            var unauthorizedResult = actionResult as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("User ID not found in claims", unauthorizedResult.Value);
        }

        [Test]
        public async Task ApplyForJob_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var job = new InternalJob
            {
                JobTitle = "Software Engineer",
                PositionId = 1,
                Description = "Develop software applications",
                Requirements = "5 years experience"
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            // Act
            var actionResult = await _controller.ApplyForJob(job.Id, "nonexistentUser");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("User not found", notFoundResult.Value);
        }

        [Test]
        public async Task ApplyForJob_ReturnsNotFound_WhenEmployeeNotFound()
        {
            // Arrange
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
                Name = "nonexistent.employee@example.com",
                Email = "nonexistent.employee@example.com",
                EmployeeId = 999 // Non-existent employee ID
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var actionResult = await _controller.ApplyForJob(job.Id, "user2");

            // Assert
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
