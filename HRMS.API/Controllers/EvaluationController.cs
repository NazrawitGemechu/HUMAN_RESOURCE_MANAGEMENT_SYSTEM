using HRMS.API.Data;
using HRMS.API.DTO;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EvaluationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        [HttpGet]
        [Route("EmployeeEvaluations")]
        public async Task<ActionResult<IEnumerable<EvaluationListDto>>> GetEmployeeEvaluations()
        {
            try
            {
                var employeeEvaluations = await _context.EmployeeEvaluations
                    .Include(e => e.Employee)
                    .ToListAsync();

                var evaluationDTOs = employeeEvaluations
                    .GroupBy(e => e.EmployeeId)
                    .Select(group => new EvaluationListDto
                    {
                        EmployeeId = group.Key,
                        EmployeeName = group.First().Employee.FirstName + " " + group.First().Employee.LastName,
                        TotalRating = group.Sum(e => e.Rating),
                        EvaluationDate = group.First().EvaluationDate 
                    }).ToList();

                return Ok(evaluationDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error fetching employee evaluations: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Detail/{employeeId}")]
        public async Task<ActionResult<EmployeeEvaluationDetailDto>> GetEmployeeEvaluationDetail(int employeeId)
        {
            try
            {
                var evaluations = await _context.EmployeeEvaluations
                    .Include(e => e.Employee)
                    .Include(e => e.EvaluationFactor)
                    .Where(e => e.EmployeeId == employeeId)
                    .ToListAsync();

                if (!evaluations.Any())
                {
                    return NotFound($"No evaluations found for employee with ID {employeeId}");
                }

                var employeeEvaluationDetail = new EmployeeEvaluationDetailDto
                {
                    EmployeeId = employeeId,
                    EmployeeName = evaluations.First().Employee.FirstName + " " + evaluations.First().Employee.LastName,
                    Evaluations = evaluations.Select(e => new EvaluationDetailDto
                    {
                        FactorName = e.EvaluationFactor.Name,
                        Rating = e.Rating,
                        EvaluationDate = e.EvaluationDate
                    }).ToList()
                };

                return Ok(employeeEvaluationDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error fetching employee evaluation detail: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{employeeId}")]
        public async Task<ActionResult<EmployeeEvaluationDto>> GetEmployeeEvaluations(int employeeId)
        {
            var evaluations = await _context.EmployeeEvaluations
                .Where(e => e.EmployeeId == employeeId)
                .Include(e => e.EvaluationFactor)
                .Select(e => new EmployeeEvaluation
                {

                    EmployeeId = e.EmployeeId,
                    EvaluationFactorId = e.EvaluationFactorId,
                    Rating = e.Rating,
                    EvaluationDate = e.EvaluationDate
                })
                .ToListAsync();

            if (evaluations == null)
            {
                return NotFound();
            }

            return Ok(evaluations);
        }

        [HttpPost]
        public async Task<IActionResult> PostEmployeeEvaluation(EmployeeEvaluationDto evaluationDto)
        {
            var employee = await _context.Employees.FindAsync(evaluationDto.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            foreach (var eval in evaluationDto.Evaluations)
            {
                var employeeEvaluation = new EmployeeEvaluation
                {
                    EmployeeId = evaluationDto.EmployeeId,
                    EvaluationFactorId = eval.FactorId,
                    Rating = eval.Rating,
                    
                };
                _context.EmployeeEvaluations.Add(employeeEvaluation);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("PostEmployeeFeedback")]
        public async Task<ActionResult<EmployeeFeedback>> PostEmployeeFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (feedbackDto == null || string.IsNullOrWhiteSpace(feedbackDto.Feedback))
            {
                return BadRequest("Invalid feedback data.");
            }

            var employee = await _context.Employees.FindAsync(feedbackDto.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var feedback = new EmployeeFeedback
            {
                EmployeeId = feedbackDto.EmployeeId,
                Feedback = feedbackDto.Feedback,
                FeedbackDate = DateTime.UtcNow
            };

            _context.EmployeeFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployeeFeedback", new { id = feedback.Id }, feedback);
        }
        [HttpGet("Employees")]
        public async Task<ActionResult<EmployeeOwnEvaluationDetailDto>> GetEmployeeEvaluationDetails([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in claims");
            }
            var user = await _userManager.FindByIdAsync(userId);
            var employee = await _context.Employees.FindAsync(user.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var evaluations = await _context.EmployeeEvaluations
                .Where(e => e.EmployeeId == employee.Id)
                .Include(e => e.EvaluationFactor)
                .ToListAsync();

            var feedbacks = await _context.EmployeeFeedbacks
                .Where(f => f.EmployeeId == employee.Id)
                .ToListAsync();

            var employeeEvaluationDetailDto = new EmployeeOwnEvaluationDetailDto
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                Evaluations = evaluations.Select(e => new EvaluationDetailDto
                {
                    FactorName = e.EvaluationFactor.Name,
                    Rating = e.Rating,
                    EvaluationDate = e.EvaluationDate
                }).ToList(),
                Feedbacks = feedbacks.Select(f => new FeedbackDetailDto
                {
                    Feedback = f.Feedback,
                    FeedbackDate = f.FeedbackDate
                }).ToList()
            };

            return Ok(employeeEvaluationDetailDto);
        }
    }
}
