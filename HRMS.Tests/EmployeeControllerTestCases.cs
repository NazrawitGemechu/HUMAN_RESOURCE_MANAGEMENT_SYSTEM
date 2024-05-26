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
    public class EmployeeControllerTestCases
    {
        private ApplicationDbContext _context;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;

        [SetUp]
        public void Setup()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed the in-memory database with test data
            SeedTestData(_context);

            // Setup mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            // Setup mock RoleManager
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
        }

        private void SeedTestData(ApplicationDbContext context)
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

            context.Departments.AddRange(departments);
            context.Grades.AddRange(grades);
            context.Positions.AddRange(positions);
            context.Branches.AddRange(branches);
            context.Degrees.AddRange(degrees);

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
                },
                new Employee
                {
                    Id = 2,
                    Emp_Id = "EMP002",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Gender = "Female",
                    MotherName = "Janet Doe",
                    PhoneNo = "0987654321",
                    Roles = "Manager",
                    Email = "jane.smith@example.com",
                    MaritalStatus = "Married",
                    Region = "Region2",
                    Woreda = "Woreda2",
                    Kebele = 2,
                    HouseNo = "House2",
                    DepartmentId = 2,
                    GradeId = 2,
                    PositionId = 2,
                    BranchId = 2,
                    DegreeId = 2,
                    HireDate = new DateTime(2021, 1, 1),
                    Salary = 60000,
                    ContactPersons = new List<ContactPerson>
                    {
                        new ContactPerson { Id = 2, Name = "Emergency Contact 2", Relationship = "Spouse", PhoneNo = "2222222222", Region = "Region2", Woreda = "Woreda2", Kebele = 2, HouseNo = "House2" }
                    },
                    Educations = new List<Education>
                    {
                        new Education { Id = 2, Degree = "MSc", Institute = "Institute2" }
                    },
                    Experiences = new List<Experience>
                    {
                        new Experience { Id = 2, CompanyName = "Company2", Position = "Senior Manager", StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2021, 1, 1) }
                    }
                }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        [Test]
        public async Task GetEmployees_ReturnsOk_WithExpectedData()
        {
            // Arrange
            var controller = new EmployeeController(_context, _mockUserManager.Object, _mockRoleManager.Object);

            // Act
            var actionResult = await controller.GetEmployees();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult.Result);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var employees = okResult.Value as IEnumerable<ListEmployeesDto>;
            Assert.IsNotNull(employees);
            Assert.AreEqual(2, employees.Count());

            // Further assertions can be added to verify the details of each employee
            var employeeList = employees.ToList();
            Assert.AreEqual("EMP001", employeeList[0].Emp_Id);
            Assert.AreEqual("John", employeeList[0].FirstName);
            Assert.AreEqual("Doe", employeeList[0].LastName);
            Assert.AreEqual("EMP002", employeeList[1].Emp_Id);
            Assert.AreEqual("Jane", employeeList[1].FirstName);
            Assert.AreEqual("Smith", employeeList[1].LastName);
        }
        [Test]
        public async Task CorrectRegisterEmployee_ValidData_ReturnsOk()
        {
            // Arrange
            var controller = new EmployeeController(_context, _mockUserManager.Object, _mockRoleManager.Object);
            var employeeDto = new UpdateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                MotherName = "Jane Doe",
                Email = "john.doe@example.com",
                Emp_Id = "EMP003",
                DepartmentId = 1,
                GradeId = 1,
                PositionId = 1,
                BranchId = 1,
                DegreeId = 1,
                HireDate = DateTime.Now,
                Salary = 50000,
                Roles = "Developer",
                Gender = "Male",
                MaritalStatus = "Single",
                Region = "Region",
                Woreda = "Woreda",
                Kebele = 123,
                PhoneNo = "1234567890",
                Experiences = new List<ExperienceDto>
    {
      new ExperienceDto
      {
        CompanyName = "ABC Company",
        ExperiencePosition = "Software Engineer",
        ExperienceStartDate = DateTime.Now.AddYears(-5),
        ExperienceEndDate = DateTime.Now.AddYears(-3)
      }
    },
                Educations = new List<EducationDto>
    {
      new EducationDto
      {
        Degree = "Bachelor",
        Institute = "University"
      }
    },
                ContactPersons = new List<ContactPersonDto>
    {
      new ContactPersonDto
      {
        ContactPersonName = "Jane Smith",
        Relationship = "Relative",
        ContactPhoneNo = "9876543210",
        ContactRegion = "Region",
        ContactWoreda = "Woreda",
        ContactKebele = 456,
        ContactHouseNo = "123"
      }
    },
                ChildInformations = new List<ChildDto>
    {
      new ChildDto
      {
        ChildName = "Alice",
        DateOfBirth = DateTime.Now.AddYears(-5)
      }
    }
            };

            // Mock UserManager CreateAsync to return success
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
              .Returns(Task.FromResult(IdentityResult.Success));

            // Mock RoleManager RoleExistsAsync to return false (need to create role)
            _mockRoleManager.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(false));

            // Mock RoleManager CreateAsync to succeed
            _mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
              .Returns(Task.FromResult(IdentityResult.Success));

            // Mock UserManager AddToRoleAsync to succeed
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
              .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var actionResult = await controller.CorrectRegisterEmployee(employeeDto);

            // Assert
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var response = okResult.Value as EmployeeDto;
            Assert.IsNotNull(response);
            // Add assertions to verify other properties of the returned EmployeeDto
        }

        [Test]
        public async Task CorrectRegisterEmployee_ReturnsBadRequest_ForDuplicateEmpId()
        {
            // Arrange
            var controller = new EmployeeController(_context, _mockUserManager.Object, _mockRoleManager.Object);
            var existingEmployee = _context.Employees.First(); // Assuming there's already an employee in the database
            var employeeDto = new UpdateEmployeeDto
            {
                Emp_Id = existingEmployee.Emp_Id, // Using existing employee's Emp_Id
                FirstName = "Bob",
                LastName = "Johnson",
                MotherName = "Mary Johnson",
                Email = "bob.johnson@example.com",
                DepartmentId = 1,
                GradeId = 1,
                PositionId = 1,
                BranchId = 1,
                DegreeId = 1,
                HireDate = DateTime.Now,
                Salary = 70000,
                Roles = "Employee",
                Gender = "Male",
                MaritalStatus = "Single",
                Woreda = "Sample Woreda",
                Kebele = 123,
                HouseNo = "123",
                Region = "Sample Region",
                PhoneNo = "0987654321",
                Experiences = new List<ExperienceDto>
        {
            new ExperienceDto
            {
                CompanyName = "ABC Company",
                ExperiencePosition = "Software Engineer",
                ExperienceStartDate = DateTime.Parse("2020-01-01"),
                ExperienceEndDate = DateTime.Parse("2022-01-01")
            }
        },
                Educations = new List<EducationDto>
        {
            new EducationDto
            {
                Degree = "Bachelor's",
                Institute = "XYZ University"
            }
        },
                ContactPersons = new List<ContactPersonDto>
        {
            new ContactPersonDto
            {
                ContactPersonName = "Alice Johnson",
                Relationship = "Spouse",
                ContactPhoneNo = "0123456789",
                ContactWoreda = "Sample Woreda",
                ContactRegion = "Sample Region",
                ContactKebele = 123,
                ContactHouseNo = "123"
            }
        },
                ChildInformations = new List<ChildDto>
        {
            new ChildDto
            {
                ChildName = "Tom Johnson",
                DateOfBirth = DateTime.Parse("2015-01-01")
            }
        }
            };

            // Act
            var actionResult = await controller.CorrectRegisterEmployee(employeeDto);

            // Assert
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            // Add more assertions to check the error response, if needed
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
