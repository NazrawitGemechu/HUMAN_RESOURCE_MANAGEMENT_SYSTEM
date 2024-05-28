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
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();

            _context.Employees.Add(new Employee
            {
                Emp_Id = "EMP001",
                FirstName = "Nazrawit",
                LastName = "Gemechu",
                Gender = "Female",
                Email = "nazrawitgemechu@gmail.com",
                PhoneNo = "0923465322",
                MotherName = "Saron",
                MaritalStatus = "Single",
                Region = "Test Region",
                Woreda = "Test Woreda",
                Roles = "Employee",
                Id = 1 
            });

            _context.Departments.Add(new Department { Id = 1, Name = "Test Department" });
            _context.Positions.Add(new Position { Id = 1, Name = "Test Position" });
            _context.SaveChanges();
            var resignation = new Resignation
            {
                Id = 1,
                EmployeeId = 1,
                FullName = "Nazrawit Gemechu",
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
        public async Task GetResignationRequests_ReturnsOk()
        {
            var _controller = new ResignationController(_context);
            var result = await _controller.GetResignationRequests() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
        }

        [Test]
        public async Task ApproveResignation_ReturnsOk_ApprovedSuccessfully()
        {
            var _controller = new ResignationController(_context);
            var result = await _controller.ApproveResignation(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Resignation request approved successfully", result.Value);
        }

        [Test]
        public async Task RejectResignation_ReturnsOk_WhenRejectedSuccessfully()
        {
            var _controller = new ResignationController(_context);
            var result = await _controller.RejectResignation(1) as OkObjectResult;

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
