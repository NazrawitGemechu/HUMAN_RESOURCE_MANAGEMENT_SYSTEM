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
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new UserManager<ApplicationUser>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            _context.Employees.Add(new Employee
            {
                Emp_Id = "1",
                FirstName = "Nazrawit",
                LastName = "Gemechu",
                Gender = "Female",
                Email = "nazrawitgemechu@gmail.com",
                PhoneNo = "0922538433",
                MotherName = "Saron",
                MaritalStatus = "Single",
                Region = "Test Region",
                Woreda = "Test Woreda",
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

            var result = await controller.PostCompliant(complaintDto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Compliant submitted successfully", result.Value);
        }

        [Test]
        public async Task AddressCompliant_ValidComplaint_ReturnsOk()
        {
            var controller = new ComplaintController(_context, _userManager);

            var result = await controller.AddressCompliant(1) as OkResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var compliant = await _context.Compliants.FindAsync(1);
            Assert.AreEqual("Addressed", compliant.Status);
        }

        [Test]
        public async Task AddressCompliant_InvalidComplaint_ReturnsNotFound()
        {
            var controller = new ComplaintController(_context, _userManager);

            var result = await controller.AddressCompliant(999) as NotFoundObjectResult;
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
