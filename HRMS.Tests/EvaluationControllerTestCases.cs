using HRMS.API.Controllers;
using HRMS.API.Data;
using HRMS.API.Models;
using HRMS.API.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Tests
{
    public class EvaluationControllerTestCases
    {
        private ApplicationDbContext _context;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            _userManagerMock = MockUserManager<ApplicationUser>();
        }

        [Test]
        public async Task GetEmployeeEvaluations_ShouldReturnOkWithEmployeeEvaluations()
        {
            // Arrange
            var controller = new EvaluationController(_context, _userManagerMock.Object);

            // Act
            var result = await controller.GetEmployeeEvaluations();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<EvaluationListDto>>());
        }

        [Test]
        public async Task GetEmployeeEvaluations_ShouldReturnInternalServerErrorOnException()
        {
            // Arrange
            _context.Dispose(); // Dispose the context to simulate an exception
            var controller = new EvaluationController(_context, _userManagerMock.Object);

            // Act
            var result = await controller.GetEmployeeEvaluations();

            // Assert
            Assert.That(result.Result, Is.TypeOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(objectResult.Value, Is.TypeOf<string>());
        }

        [Test]
        public async Task GetEvaluationDetail_ReturnNotFound_WhenEmployeeNotFound()
        {
            // Arrange
            var employeeId = 1;
            var controller = new EvaluationController(_context, _userManagerMock.Object);

            // Act
            var result = await controller.GetEmployeeEvaluationDetail(employeeId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [TearDown]
        public void DisposeContext()
        {
            _context.Dispose();
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            return userManager;
        }
    }
}
