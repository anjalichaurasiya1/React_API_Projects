using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using product_api.Models;
using product_api.Service;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendenceController : ControllerBase
    {
        private readonly AttendenceService _attendenceService;

        public AttendenceController(AttendenceService attendenceService)
        {
            _attendenceService = attendenceService;
        }

        // GET api/attendence/monthly?empId=1&month=3&year=2026
        [HttpGet("monthly")]
        public async Task<IActionResult> GetEmployeeMonthlyAttendance(int empId, int month, int year)
        {
            var result = await _attendenceService.GetEmployeeMonthlyAttendanceAsync(empId, month, year);

            return Ok(result);
        }

    }
}