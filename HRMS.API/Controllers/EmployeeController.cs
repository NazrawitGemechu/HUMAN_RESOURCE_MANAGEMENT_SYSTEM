using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HRMS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet("Filter")]
        public IActionResult FilterEmployees(string empId = null, string name = null, string department = null, string position = null, string email = null, string branch = null)
        {
            IQueryable<Employee> query = _context.Employees;

            if (!string.IsNullOrEmpty(empId))
            {
                query = query.Where(e => e.Emp_Id == empId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name));
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department.Name == department);
            }

            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(e => e.Position.Name == position);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(e => e.Email == email);
            }

            if (!string.IsNullOrEmpty(branch))
            {
                query = query.Where(e => e.Branch.Name == branch);
            }

            var employees = query.ToList();

            return Ok(employees);
        }

        [HttpGet("Download")]
        public IActionResult DownloadEmployees(string empId = null, string name = null, string department = null, string position = null, string email = null, string branch = null)
        {
            IQueryable<Employee> query = _context.Employees
    .Include(e => e.Department)
    .Include(e => e.Position)
    .Include(e => e.Branch);

            if (!string.IsNullOrEmpty(empId))
            {
                query = query.Where(e => e.Emp_Id == empId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name));
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department.Name == department);
            }

            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(e => e.Position.Name == position);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(e => e.Email == email);
            }

            if (!string.IsNullOrEmpty(branch))
            {
                query = query.Where(e => e.Branch.Name == branch);
            }

            var employees = query.ToList();

            // Create CSV content
            var csv = new StringBuilder();
            csv.AppendLine("Emp_Id,Name,Department,Position,Email,Branch,HireDate,PhoneNo,Gender,Salary,Roles,Status");
            foreach (var employee in employees)
            {
                csv.AppendLine($"{employee.Emp_Id},{employee.FirstName} {employee.LastName},{employee.Department.Name},{employee.Position.Name},{employee.Email},{employee.Branch.Name},{employee.HireDate},{employee.PhoneNo},{employee.Gender},{employee.Salary},{employee.Roles},{employee.Status}");
            }

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

            return File(memoryStream, "text/csv", "employees.csv");
        }

        [HttpGet]
        [Route("ListEmployees")]
        public async Task<ActionResult<IEnumerable<ListEmployeesDto>>> GetEmployees()
        {
            var employees = await _context.Employees
                                            .Select(e => new ListEmployeesDto
                                            {
                                                Id = e.Id,
                                                Emp_Id = e.Emp_Id,
                                                FirstName = e.FirstName,
                                                LastName = e.LastName,
                                                Gender = e.Gender,
                                                MotherName = e.MotherName,
                                                PhoneNo = e.PhoneNo,
                                                Role = e.Roles,

                                            })
                                            .ToListAsync();

            return Ok(employees);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UpdateEmployeeDto>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Educations)
                .Include(e => e.Experiences)
                .Include(e => e.ContactPersons)
                .Include(e => e.ChildInformations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeDto = new UpdateEmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MotherName = employee.MotherName,
                Email = employee.Email,
                Gender = employee.Gender,
                MaritalStatus = employee.MaritalStatus,
                Emp_Id = employee.Emp_Id,
                Region = employee.Region,
                Woreda = employee.Woreda,
                Kebele = employee.Kebele,
                HouseNo = employee.HouseNo,
                PhoneNo = employee.PhoneNo,
                DepartmentId = employee.DepartmentId,
                GradeId = employee.GradeId,
                PositionId = employee.PositionId,
                BranchId = employee.BranchId,
                DegreeId = employee.DegreeId,
                HireDate = employee.HireDate,
                Salary = employee.Salary,
                Roles = employee.Roles,
                Educations = employee.Educations.Select(edu => new EducationDto
                {
                    Degree = edu.Degree,
                    Institute = edu.Institute
                }).ToList(),
                Experiences = employee.Experiences.Select(exp => new ExperienceDto
                {
                    CompanyName = exp.CompanyName,
                    ExperiencePosition = exp.Position,
                    ExperienceStartDate = exp.StartDate,
                    ExperienceEndDate = exp.EndDate
                }).ToList(),
                ContactPersons = employee.ContactPersons.Select(cp => new ContactPersonDto
                {
                    ContactPersonName = cp.Name,
                    Relationship = cp.Relationship,
                    ContactPhoneNo = cp.PhoneNo,
                    ContactRegion = cp.Region,
                    ContactWoreda = cp.Woreda,
                    ContactKebele = cp.Kebele,
                    ContactHouseNo = cp.HouseNo
                }).ToList(),
                ChildInformations = employee.ChildInformations.Select(child => new ChildDto
                {
                    ChildName = child.Name,
                    DateOfBirth = child.DateOfBirth ?? DateTime.MinValue
                }).ToList()
            };

            return employeeDto;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
        {


            var employee = await _context.Employees
                .Include(e => e.Educations)
                .Include(e => e.Experiences)
                .Include(e => e.ContactPersons)
                .Include(e => e.Promotions)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Update employee properties
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.MotherName = employeeDto.MotherName;
            employee.Email = employeeDto.Email;
            employee.Gender = employeeDto.Gender;
            employee.MaritalStatus = employeeDto.MaritalStatus;
            employee.Emp_Id = employeeDto.Emp_Id;
            employee.Woreda = employeeDto.Woreda;
            employee.Kebele = employeeDto.Kebele;
            employee.HouseNo = employeeDto.HouseNo;
            employee.PhoneNo = employeeDto.PhoneNo;
            employee.Region = employeeDto.Region;
            employee.DepartmentId = employeeDto.DepartmentId;
            employee.GradeId = employeeDto.GradeId;
            employee.PositionId = employeeDto.PositionId;
            employee.BranchId = employeeDto.BranchId;
            employee.DegreeId = employeeDto.DegreeId;
            employee.HireDate = employeeDto.HireDate;
            employee.Salary = employeeDto.Salary;
            employee.Roles = employeeDto.Roles;

            _context.Entry(employee).State = EntityState.Modified;

            // Update related entities (Educations)
            if (employeeDto.Educations != null)
            {
                // Remove existing educations
                _context.Educations.RemoveRange(employee.Educations);

                // Add new educations
                foreach (var educationDto in employeeDto.Educations)
                {
                    var education = new Education
                    {
                        Degree = educationDto.Degree,
                        Institute = educationDto.Institute,
                        EmployeeId = employee.Id
                    };
                    _context.Educations.Add(education);
                }
            }

            // Update related entities (Experiences)
            if (employeeDto.Email != null)
            {
                var user = new ApplicationUser
                {
                    Email = employeeDto.Email,
                };
                _context.Entry(user).State = EntityState.Modified;
            }

            if (employeeDto.Experiences != null)
            {
                // Remove existing experiences
                _context.Experiences.RemoveRange(employee.Experiences);

                // Add new experiences
                foreach (var experienceDto in employeeDto.Experiences)
                {
                    var experience = new Experience
                    {
                        CompanyName = experienceDto.CompanyName,
                        Position = experienceDto.ExperiencePosition,
                        StartDate = experienceDto.ExperienceStartDate,
                        EndDate = experienceDto.ExperienceEndDate,
                        EmployeeId = employee.Id
                    };
                    _context.Experiences.Add(experience);
                }
            }

            // Update related entities (ContactPersons)
            if (employeeDto.ContactPersons != null)
            {
                // Remove existing contact persons
                _context.ContactPersons.RemoveRange(employee.ContactPersons);

                // Add new contact persons
                foreach (var contactPersonDto in employeeDto.ContactPersons)
                {
                    var contactPerson = new ContactPerson
                    {
                        Name = contactPersonDto.ContactPersonName,
                        Relationship = contactPersonDto.Relationship,
                        PhoneNo = contactPersonDto.ContactPhoneNo,
                        Region = contactPersonDto.ContactRegion,
                        Kebele = contactPersonDto.ContactKebele,
                        Woreda = contactPersonDto.ContactWoreda,
                        HouseNo = contactPersonDto.ContactHouseNo,
                        EmployeeId = employee.Id
                    };
                    _context.ContactPersons.Add(contactPerson);
                }
            }
            // Update related entities (ChildInformations)
            if (employeeDto.ChildInformations != null)
            {
                // Remove existing contact persons
                _context.ChildInformations.RemoveRange(employee.ChildInformations);

                // Add new contact persons
                foreach (var childDto in employeeDto.ChildInformations)
                {
                    var childDetail = new ChildInformation
                    {
                        Name = childDto.ChildName,
                        DateOfBirth = childDto.DateOfBirth,
                        EmployeeId = employee.Id
                    };
                    _context.ChildInformations.Add(childDetail);
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(employeeDto);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        [HttpDelete]
        [Route("DeleteEmployee/{Id}")]
        public async Task<IActionResult> DeleteEmployee(int Id)
        {
            var employee = await _context.Employees
        .Include(e => e.Educations)
        .Include(e => e.Experiences)
        .Include(e => e.ContactPersons)
        .Include(e => e.ChildInformations)
        .FirstOrDefaultAsync(e => e.Id == Id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            _context.Educations.RemoveRange(employee.Educations);
            _context.Experiences.RemoveRange(employee.Experiences);
            _context.ContactPersons.RemoveRange(employee.ContactPersons);
            _context.ChildInformations.RemoveRange(employee.ChildInformations);
            var user = await _userManager.FindByEmailAsync(employee.Email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                var toDelete = await _context.Employees
                                                .Include(e => e.Educations)
                                                .Include(e => e.Experiences)
                                                .Include(e => e.ContactPersons)
                                                .Include(e => e.ChildInformations)
                                                .FirstOrDefaultAsync(e => e.Id == Id);
                _context.Employees.Remove(toDelete);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                throw new Exception("Failed to delete user");
            }
        }


        [HttpPost]
        [Route("CorrectRegisterEmployee")]
        public async Task<IActionResult> CorrectRegisterEmployee([FromBody] UpdateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (_context.Employees.Any(e => e.Emp_Id == employeeDto.Emp_Id))
                {
                    return BadRequest("Employee ID already exists. Please choose another.");
                }

                var employee = new Employee
                {
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    MotherName = employeeDto.MotherName,
                    Email = employeeDto.Email,
                    Emp_Id = employeeDto.Emp_Id,
                    DepartmentId = employeeDto.DepartmentId,
                    GradeId = employeeDto.GradeId,
                    PositionId = employeeDto.PositionId,
                    BranchId = employeeDto.BranchId,
                    DegreeId = employeeDto.DegreeId,
                    HireDate = employeeDto.HireDate,
                    Salary = employeeDto.Salary,
                    Roles = employeeDto.Roles,
                    Gender = employeeDto.Gender,
                    MaritalStatus = employeeDto.MaritalStatus,
                    Woreda = employeeDto.Woreda,
                    Kebele = employeeDto.Kebele,
                    HouseNo = employeeDto.HouseNo,
                    Region = employeeDto.Region,
                    PhoneNo = employeeDto.PhoneNo,
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Add experience details
                foreach (var expDto in employeeDto.Experiences)
                {
                    var experience = new Experience
                    {
                        EmployeeId = employee.Id,
                        CompanyName = expDto.CompanyName,
                        Position = expDto.ExperiencePosition,
                        StartDate = expDto.ExperienceStartDate,
                        EndDate = expDto.ExperienceEndDate
                    };
                    _context.Experiences.Add(experience);
                }

                // Add education details
                foreach (var eduDto in employeeDto.Educations)
                {
                    var education = new Education
                    {
                        EmployeeId = employee.Id,
                        Degree = eduDto.Degree,
                        Institute = eduDto.Institute
                    };
                    _context.Educations.Add(education);
                }

                // Add contact person details
                foreach (var contactDto in employeeDto.ContactPersons)
                {
                    var contactPerson = new ContactPerson
                    {
                        EmployeeId = employee.Id,
                        Name = contactDto.ContactPersonName,
                        Relationship = contactDto.Relationship,
                        PhoneNo = contactDto.ContactPhoneNo,
                        Woreda = contactDto.ContactWoreda,
                        Region = contactDto.ContactRegion,
                        Kebele = contactDto.ContactKebele,
                        HouseNo = contactDto.ContactHouseNo,
                    };
                    _context.ContactPersons.Add(contactPerson);
                }

                // Add child details
                foreach (var childDto in employeeDto.ChildInformations)
                {
                    var childDetail = new ChildInformation
                    {
                        EmployeeId = employee.Id,
                        Name = childDto.ChildName,
                        DateOfBirth = childDto.DateOfBirth,
                    };
                    _context.ChildInformations.Add(childDetail);
                }

                await _context.SaveChangesAsync();

                var password = employeeDto.FirstName + "@123";
                var username = employeeDto.Emp_Id;

                var user = new ApplicationUser
                {
                    Name = employeeDto.FirstName,
                    Email = employeeDto.Email,
                    EmailConfirmed = true,
                    UserName = username,
                    EmployeeId = employee.Id
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(employeeDto.Roles);
                    if (!roleExists)
                    {
                        var role = new IdentityRole(employeeDto.Roles);
                        await _roleManager.CreateAsync(role);
                    }

                    await _userManager.AddToRoleAsync(user, employeeDto.Roles);

                    var response = new EmployeeDto
                    {
                        Id = employee.Id,
                        Email = employee.Email,
                        Emp_Id = employee.Emp_Id,
                        DepartmentId = employee.DepartmentId,
                        GradeId = employee.GradeId,
                        PositionId = employee.PositionId,
                        BranchId = employee.BranchId,
                        DegreeId = employee.DegreeId,
                        HireDate = employee.HireDate,
                        Salary = employee.Salary,
                        Roles = employee.Roles
                    };

                    return Ok(response);
                }
                else
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to register employee: " + ex.InnerException?.Message);
            }
        }

        [HttpPost]
        [Route("UploadEmployeePhoto")]
        public async Task<IActionResult> UploadEmployeePhoto(EmployeePhotoDto uploadModel)
        {
            if (uploadModel.Photo == null || uploadModel.Photo.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var employee = await _context.Employees.FindAsync(uploadModel.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var fileName = Path.GetFileName(uploadModel.Photo.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadModel.Photo.CopyToAsync(stream);
            }

            var employeePhoto = new EmployeePhoto
            {
                EmployeeId = uploadModel.EmployeeId,
                pictureURL = "/images/" + fileName
            };
            _context.EmployeePhotos.Add(employeePhoto);
            await _context.SaveChangesAsync();

            return Ok("Employee photo uploaded successfully");
        }
        [HttpPost]
        [Route("UpdateEmployeePhoto")]
        public async Task<IActionResult> UpdateEmployeePhoto(EmployeePhotoDto updateModel)
        {
            if (updateModel.Photo == null || updateModel.Photo.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            var employee = await _context.Employees.FindAsync(updateModel.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var existingPhoto = await _context.EmployeePhotos.FirstOrDefaultAsync(ep => ep.EmployeeId == updateModel.EmployeeId);
            if (existingPhoto != null)
            {
                _context.EmployeePhotos.Remove(existingPhoto);
            }

            var fileName = Path.GetFileName(updateModel.Photo.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await updateModel.Photo.CopyToAsync(stream);
            }

            var employeePhoto = new EmployeePhoto
            {
                EmployeeId = updateModel.EmployeeId,
                pictureURL = "/images/" + fileName
            };
            _context.EmployeePhotos.Add(employeePhoto);
            await _context.SaveChangesAsync();

            return Ok("Employee photo updated successfully");
        }

        [HttpPost]
        [Route("DeleteEmployeePhoto")]
        public async Task<IActionResult> DeleteEmployeePhoto(int employeeId)
        {

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var existingPhoto = await _context.EmployeePhotos.FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId);
            if (existingPhoto != null)
            {
                _context.EmployeePhotos.Remove(existingPhoto);
                await _context.SaveChangesAsync();
                return Ok("Employee photo deleted successfully");
            }

            return NotFound("Employee photo not found");
        }
    }
}
