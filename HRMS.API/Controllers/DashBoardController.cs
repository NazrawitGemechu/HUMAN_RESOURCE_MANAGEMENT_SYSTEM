using HRMS.API.Data;
using HRMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HRMS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashBoardController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("ActiveCount")]
        public async Task<IActionResult> GetActiveEmployeeCount()
        {
            try
            {
                var activeEmployeeCount = await _context.Employees
                    .Where(e => e.Status == "Active")
                    .CountAsync();
                return Ok(activeEmployeeCount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving active employee count: {ex.Message}");
            }
        }
        [HttpGet("InactiveCount")]
        public async Task<IActionResult> GetInactiveEmployeeCount()
        {
            try
            {
                var inactiveEmployeeCount = await _context.Employees
                    .Where(e => e.Status == "Inactive")
                    .CountAsync();
                return Ok(inactiveEmployeeCount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving inactive employee count: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("CountPendingLeaveRequests")]
        public async Task<IActionResult> CountPendingLeaveRequests()
        {
            var pendingCount = await _context.Leaves.CountAsync(l => l.Status == "Pending");
            return Ok(pendingCount);
        }

        [HttpGet]
        [Route("CountApprovedLeaveRequests")]
        public async Task<IActionResult> CountApprovedLeaveRequests()
        {
            var approvedCount = await _context.Leaves.CountAsync(l => l.Status == "Approved");
            return Ok(approvedCount);
        }

        [HttpGet]
        [Route("CountRejectedLeaveRequests")]
        public async Task<IActionResult> CountRejectedLeaveRequests()
        {
            var rejectedCount = await _context.Leaves.CountAsync(l => l.Status == "Rejected");
            return Ok(rejectedCount);
        }
        [HttpGet]
        [Route("TotalEmployeesPerPosition")]
        public IActionResult TotalEmployeesPerPosition()
        {
            var employeesPerPosition = _context.Employees
                .GroupBy(e => e.PositionId)
                .Select(g => new
                {
                    PositionId = g.Key,
                    PositionName = g.FirstOrDefault().Position.Name,
                    TotalEmployees = g.Count()
                })
                .ToList();

            return Ok(employeesPerPosition);
        }

        [HttpGet]
        [Route("TotalEmployeesPerBranch")]
        public IActionResult TotalEmployeesPerBranch()
        {
            var employeesPerBranch = _context.Employees
                .GroupBy(e => e.BranchId)
                .Select(g => new
                {

                    BranchName = g.FirstOrDefault().Branch.Name,
                    TotalEmployees = g.Count()
                })
                .ToList();

            return Ok(employeesPerBranch);
        }
        [HttpGet]
        [Route("EmployeesPerPosition")]
        public IActionResult EmployeesPerPosition()
        {
            var employeesPerPosition = _context.Employees
                .GroupBy(e => e.PositionId)
                .Select(g => new
                {
                    PositionId = g.Key,
                    PositionName = g.FirstOrDefault().Position.Name,
                    Employees = g.Select(e => new
                    {
                        e.Id,
                        e.FirstName,
                        e.LastName,
                        e.Email,
                    }).ToList()
                })
                .ToList();

            return Ok(employeesPerPosition);
        }

        [HttpGet]
        [Route("EmployeesPerBranch")]
        public IActionResult EmployeesPerBranch()
        {
            var employeesPerBranch = _context.Employees
                .GroupBy(e => e.BranchId)
                .Select(g => new
                {
                    BranchId = g.Key,
                    BranchName = g.FirstOrDefault().Branch.Name,
                    Employees = g.Select(e => new
                    {
                        e.Id,
                        e.FirstName,
                        e.LastName,
                        e.Email,
                    }).ToList()
                })
                .ToList();

            return Ok(employeesPerBranch);
        }
        [HttpGet("DownloadEmployeesByBranch")]
        public async Task<IActionResult> DownloadEmployeesByBranch(int branchId)
        {
            var employees = await _context.Employees
                .Where(e => e.BranchId == branchId)
                .ToListAsync();
            var csvData = FormatEmployeesToCsv(employees);
            var csvBytes = Encoding.UTF8.GetBytes(csvData);
            return File(csvBytes, "text/csv", "employees.csv");
        }

        private string FormatEmployeesToCsv(IEnumerable<Employee> employees)
        {
            var csv = new StringBuilder();
            var propertyNames = typeof(Employee).GetProperties().Select(p => p.Name);

            csv.AppendLine(string.Join(",", propertyNames));

            foreach (var employee in employees)
            {
                var rowValues = propertyNames.Select(propertyName => FormatValue(employee.GetType().GetProperty(propertyName).GetValue(employee)));
                csv.AppendLine(string.Join(",", rowValues));
            }

            return csv.ToString();
        }


        [HttpGet("EmployeesHiredPerYear")]
        public async Task<IActionResult> GetEmployeesHiredPerYear()
        {
            var employeesHiredPerYear = await _context.Employees
                .GroupBy(e => e.HireDate.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(employeesHiredPerYear);
        }

        private string FormatValue(object value)
        {
            return value != null ? value.ToString() : "";
        }
    }
}
