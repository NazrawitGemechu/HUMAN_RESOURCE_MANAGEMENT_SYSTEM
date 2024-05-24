using HRMS.API.Controllers;
using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Tests
{
    [TestFixture]
    public class CompliantControllerTestCases
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        [SetUp]
        public void Setup()
        {
            // Mocking DbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);

            // Mocking UserManager
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new UserManager<ApplicationUser>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            // Seed data
            // Seed data
            _context.Employees.Add(new Employee
            {
                Emp_Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Email = "john.doe@example.com",
                PhoneNo = "1234567890",
                MotherName = "Jane Doe",
                MaritalStatus = "Single",
                Region = "Region1",
                Woreda = "Woreda1",
                Roles = "Employee"
            });


            _context.Branches.Add(new Branch { Id = 1 ,Name="Chacha"});
            _context.Positions.Add(new Position { Id = 1 ,Name="Manager"});
            _context.Compliants.Add(new Compliant
            {
                Id = 1,
                Name = "Test Compliant",
                EmployeeId = 1,
                PositionId = 1,
                BranchId = 1,
                Remedy = "Test Remedy",
                Incident = "Test Incident",
                DateOfEvent = DateTime.Now,
                Status = "Pending"
            });
            _context.SaveChanges();
        }

        [Test]
        public async Task PostCompliant_ValidComplaint_ReturnsOk()
        {
            // Arrange
            var controller = new ComplaintController(_context, _userManager);
            var complaintDto = new ComplaintDto
            {
                Name = "Test Complaint",
                Emp_Id = "1",
                BranchId = 1,
                PositionId = 1,
                Remedy = "Test Remedy",
                Incident = "Test Incident",
                DateOfEvent = DateTime.Now
            };

            // Act
            var result = await controller.PostCompliant(complaintDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Compliant submitted successfully", result.Value);
        }

        [Test]
        public async Task AddressCompliant_ValidComplaint_ReturnsOk()
        {
            // Arrange
            var controller = new ComplaintController(_context, _userManager);

            // Act
            var result = await controller.AddressCompliant(1) as OkResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var compliant = await _context.Compliants.FindAsync(1);
            Assert.AreEqual("Addressed", compliant.Status);
        }

        [Test]
        public async Task AddressCompliant_InvalidComplaint_ReturnsNotFound()
        {
            // Arrange
            var controller = new ComplaintController(_context, _userManager);

            // Act
            var result = await controller.AddressCompliant(999) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Complaint not found", result.Value);
        }
        
        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [TearDown]
        public void Dispose()
        {
            _userManager?.Dispose();
        }
    }
}
