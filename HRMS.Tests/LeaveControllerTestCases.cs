using HRMS.API.Controllers;
using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Tests
{
    [TestFixture]
    public class LeaveControllerTestCases
    {
        private ApplicationDbContext _context;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<IConfiguration> _mockConfiguration;
        private LeaveController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            _mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockConfiguration = new Mock<IConfiguration>();

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

            var leaveType = new LeaveType
            {
                Id = 1,
                Name = "Annual Leave",
                AllowedDays = 20
            };

            var leave = new Leave
            {
                Id = 1,
                EmployeeId = employee.Id,
                Employee = employee,
                LeaveTypeId = leaveType.Id,
                LeaveType = leaveType,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5),
                Reason = "Vacation",
                Status = "Pending"
            };

            _context.Employees.Add(employee);
            _context.LeaveTypes.Add(leaveType);
            _context.Leaves.Add(leave);
            _context.SaveChanges();

            // Initialize the controller
            _controller = new LeaveController(_context, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object);
        }

        private async Task<int> CalculateRemainingLeaveBalance(int employeeId, int leaveTypeId)
        {
            // Mock implementation of the remaining leave balance logic
            return 15;
        }

        private async Task UpdateLeaveBalance(int employeeId, int leaveTypeId, int remainingBalance)
        {
            // Mock implementation of the update leave balance logic
        }

        [Test]
        public async Task RequestLeave_ReturnsOk_WhenLeaveRequestIsValid()
        {
            // Arrange
            var leaveRequestDto = new LeaveRequestDto
            {
                Emp_Id = "EMP001",
                LeaveTypeId = 1,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5),
                Reason = "Vacation"
            };

            // Act
            var actionResult = await _controller.RequestLeave(leaveRequestDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Leave request submitted successfully", okResult.Value);
        }

        [Test]
        public async Task RequestLeave_ReturnsNotFound_WhenEmployeeNotFound()
        {
            // Arrange
            var leaveRequestDto = new LeaveRequestDto
            {
                Emp_Id = "EMP999",
                LeaveTypeId = 1,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5),
                Reason = "Vacation"
            };

            // Act
            var actionResult = await _controller.RequestLeave(leaveRequestDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Employee not found", notFoundResult.Value);
        }

        [Test]
        public async Task ApproveLeave_ReturnsNotFound_WhenLeaveNotFound()
        {
            // Arrange
            var leaveId = 999; // Non-existing leaveId

            // Act
            var actionResult = await _controller.ApproveLeave(leaveId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Leave not found", notFoundResult.Value);
        }

        [Test]
        public async Task RejectLeave_ReturnsOk_WhenLeaveIsRejected()
        {
            // Arrange
            var leaveId = 1;

            // Act
            var actionResult = await _controller.RejectLeave(leaveId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Leave request rejected successfully", okResult.Value);

            var leave = await _context.Leaves.FindAsync(leaveId);
            Assert.AreEqual("Rejected", leave.Status);

            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.EmployeeId == leave.EmployeeId);
            Assert.IsNotNull(notification);
            Assert.AreEqual("Leave Request", notification.Title);
            Assert.AreEqual("Your leave request have been rejected.Contact HR for more information", notification.Message);
        }

        [Test]
        public async Task RejectLeave_ReturnsNotFound_WhenLeaveNotFound()
        {
            // Arrange
            var leaveId = 999; // Non-existing leaveId

            // Act
            var actionResult = await _controller.RejectLeave(leaveId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Leave not found", notFoundResult.Value);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
