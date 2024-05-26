using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PromotionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [Route("PostJob")]
        public async Task<IActionResult> PostJob(InternalJobDto jobDto)
        {
            var job = new InternalJob
            {
                JobTitle = jobDto.JobTitle,
                PositionId = jobDto.PositionId,
                Description = jobDto.Description,
                Requirements = jobDto.Requirements
            };
            _context.InternalJobs.Add(job);
            await _context.SaveChangesAsync();

            return Ok("Job posted successfully");
        }

        [HttpPost]
        [Route("ApplyForJob/{jobId}")]
        public async Task<IActionResult> ApplyForJob(int jobId, [FromQuery] string userId)
        {
            // Get the current user's ID from the claims
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in claims");
            }

            // Retrieve the employee ID from the user model based on the user ID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Use the employee ID from the user model to find the corresponding employee
            var employee = await _context.Employees
                .Include(e => e.AppliedJobs)
                .FirstOrDefaultAsync(e => e.Id == user.EmployeeId);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            // Create a new instance of EmployeeJobApplication
            var employeeJobApplication = new EmployeeJobApplication
            {
                EmployeeId = employee.Id,
                InternalJobId = jobId
            };

            // Add the employeeJobApplication to the employee's AppliedJobs collection
            employee.AppliedJobs.Add(employeeJobApplication);

            // Save changes to the database
            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                Title = "Job Application",
                Message = $"Your have successfully applied for the job. We will notify you on our decision.",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = employee.Id
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return Ok("Job application submitted successfully");
        }

        [HttpGet]
        [Route("PostedJobs")]
        public async Task<IActionResult> GetPostedJobs()
        {
            var jobs = await _context.InternalJobs.ToListAsync();
            return Ok(jobs);
        }

        [HttpGet]
        [Route("JobCandidates/{jobId}")]
        public async Task<IActionResult> GetJobCandidates(int jobId)
        {
            var job = await _context.InternalJobs.FindAsync(jobId);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            var candidates = await _context.EmployeeJobApplications
     .Include(app => app.Employee)
     .Where(app => app.InternalJobId == jobId)
     .Select(app => new {
         Id = app.Employee.Id,
         EmployeeId = app.Employee.Emp_Id,
         EmployeeName = app.Employee.FirstName,
         EmployeeLastName = app.Employee.LastName,
         EmployeeRole = app.Employee.Roles,
         EmployeeNo = app.Employee.PhoneNo,
         EmployeeEmail = app.Employee.Email,

     })
     .ToListAsync();

            return Ok(candidates);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<InternalJob>> GetJobById(int id)
        {
            var job = await _context.InternalJobs
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (job == null)
            {
                return NotFound();
            }
            var result = new
            {
                job.Id,
                job.JobTitle,
                PositionName = job.Position.Name,
                job.Description,
                job.Requirements,
                job.PostingDate,

            };
            return Ok(result);
        }


        [HttpPost]
        [Route("ShortlistCandidate/{employeeId}")]
        public async Task<IActionResult> ShortlistCandidate(int employeeId)
        {
            var application = await _context.EmployeeJobApplications
                .FirstOrDefaultAsync(app => app.EmployeeId == employeeId);

            if (application == null)
            {
                return NotFound("Application not found");
            }
            application.Shortlisted = true;
            application.Status = "Shortlisted";

            await _context.SaveChangesAsync();
            var notification = new Notification
            {
                Title = "Shortlisted",
                Message = $"Congratulations! You have been shortlisted.We will notify you on further progress.",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = employeeId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();


            return Ok("Candidate shortlisted successfully");
        }

        [HttpPost]
        [Route("RejectCandidate/{employeeId}")]
        public async Task<IActionResult> RejectCandidate(int employeeId)
        {
            var application = await _context.EmployeeJobApplications
                .FirstOrDefaultAsync(app => app.EmployeeId == employeeId);

            if (application == null)
            {
                return NotFound("Application not found");
            }
            application.Status = "Rejected";

            await _context.SaveChangesAsync();


            var notification = new Notification
            {
                Title = "Rejected",
                Message = $"We are sorry to inform you that you were not selected for promotion.",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = employeeId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok("Candidate rejected successfully");
        }
        [HttpGet("ShortlistedCandidates")]
        public async Task<IActionResult> GetShortlistedCandidates()
        {
            var shortlistedCandidates = await _context.EmployeeJobApplications
                .Where(app => app.Shortlisted)
                .Select(app => new
                {
                    Employee = new
                    {
                        app.Employee.Id,
                        app.Employee.Emp_Id,
                        app.Employee.FirstName,
                        app.Employee.LastName,
                        app.Employee.MotherName,
                        app.Employee.Email,
                        app.Employee.Gender,
                        app.Employee.PhoneNo,
                        app.Employee.DepartmentId,
                        app.Employee.GradeId,
                        app.Employee.PositionId,
                        app.Employee.BranchId,
                        app.Employee.DegreeId,
                        app.Employee.HireDate,
                        app.Employee.Salary,
                        app.Employee.Roles,

                        Educations = app.Employee.Educations.Select(edu => new
                        {

                            edu.Degree,
                            edu.Institute,

                        }),
                        Experiences = app.Employee.Experiences.Select(exp => new
                        {

                            exp.CompanyName,
                            exp.Position,
                            exp.StartDate,
                            exp.EndDate
                        })
                    }
                })
                .ToListAsync();

            return Ok(shortlistedCandidates);
        }

        [HttpPost("PromoteEmployee")]
        public async Task<IActionResult> PromoteEmployee(PromotionDto promotionDTO)
        {
            // Retrieve the selected shortlisted employee and the corresponding job application
            var jobApplication = await _context.EmployeeJobApplications
                .Include(app => app.Employee)
                .Include(app => app.InternalJob)
                    .ThenInclude(job => job.Position)
                .FirstOrDefaultAsync(app => app.Employee.Emp_Id == promotionDTO.EmpId && app.Shortlisted);

            if (jobApplication == null || jobApplication.Employee == null)
            {
                return NotFound("Employee not found or not shortlisted.");
            }

            // Extract necessary details
            var employeeId = jobApplication.Employee.Id;
            var previousPositionId = jobApplication.Employee.PositionId;
            var newPositionId = jobApplication.InternalJob.PositionId;

            // Create a new Promotion object
            var promotion = new Promotion
            {
                EmployeeId = employeeId,
                PreviousPositionId = previousPositionId,
                NewPositionId = newPositionId,
                NewGradeId = promotionDTO.NewGradeId,
                PromotionDate = DateTime.Now,
                Reason = promotionDTO.Reason,
                NewSalary = promotionDTO.NewSalary
            };

            // Save promotion to the database
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            // Update employee information
            var employeeToUpdate = await _context.Employees.FindAsync(employeeId);
            if (employeeToUpdate != null)
            {
                employeeToUpdate.PositionId = newPositionId;
                employeeToUpdate.GradeId = promotionDTO.NewGradeId;
                employeeToUpdate.Salary = promotionDTO.NewSalary;
                await _context.SaveChangesAsync();
            }

            // Update job application status
            jobApplication.Status = "Promoted";
            await _context.SaveChangesAsync();


            var notification = new Notification
            {
                Title = "Promoted",
                Message = $"Congratulations! Your have been promoted.",
                Timestamp = DateTime.Now,
                IsRead = false,
                EmployeeId = employeeId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok("Employee promoted successfully.");
        }

    }
}
