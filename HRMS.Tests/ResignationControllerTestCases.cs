using HRMS.API.Controllers;
using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
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
    public class ResignationControllerTestCases
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
            
            // Seed data
            _context.Employees.Add(new Employee
            {
                Emp_Id = "EMP001",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Email = "john.doe@example.com",
                PhoneNo = "1234567890",
                MotherName = "Jane Doe",
                MaritalStatus = "Single",
                Region = "Region1",
                Woreda = "Woreda1",
                Roles = "Employee",
                Id = 1 
            });

            _context.Departments.Add(new Department { Id = 1, Name = "IT" });
            _context.Positions.Add(new Position { Id = 1, Name = "Manager" });
            _context.SaveChanges();
            var resignation = new Resignation
            {
                Id = 1,
                EmployeeId = 1,
                FullName = "John Doe",
                SeparationDate = DateTime.Now.AddDays(30),
                Reason = "Personal reasons",
                Satisfaction = "Neutral",
                EmployeeRelationship = "Good",
                Recommendation = "Recommendable",
                Comment = "No comments",
                EmployeeHireDate = DateTime.Now.AddYears(-2),
                Status = "Pending",
                DepartmentId = 1,
                PositionId = 1,
            };

            _context.Add(resignation);
            _context.SaveChanges();

        }

        [Test]
        public async Task GetResignationRequests_ReturnsOk_WithResignationRequests()
        {
            var _controller = new ResignationController(_context);
            // Act
            var result = await _controller.GetResignationRequests() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
        }

        [Test]
        public async Task ApproveResignation_ReturnsOk_WhenResignationIsApprovedSuccessfully()
        {
            var _controller = new ResignationController(_context);
            // Act
            var result = await _controller.ApproveResignation(1) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Resignation request approved successfully", result.Value);
        }

        [Test]
        public async Task RejectResignation_ReturnsOk_WhenResignationIsRejectedSuccessfully()
        {
            var _controller = new ResignationController(_context);
            // Act
            var result = await _controller.RejectResignation(1) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Resignation request rejected successfully", result.Value);
        }

        [TearDown]
        public void TearDown()
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
