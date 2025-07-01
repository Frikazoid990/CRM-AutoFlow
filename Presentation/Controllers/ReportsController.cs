using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [Route("reports")]
    [ApiController]
    //[Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;

        public ReportsController (IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpPost("/reports/car-report")]
        public async Task<IActionResult> CarReport([FromBody] DateRangeDto Date)
        {
            var result = await _reportsService.ReportCar(Date.DateStart, Date.DateEnd);
            return Ok(result);
        }

        [HttpPost("/reports/manager-report")]
        public async Task<IActionResult> ManagerReport([FromBody] DateRangeDto Date)
        {
            var result = await _reportsService.ReportManager(Date.DateStart, Date.DateEnd);
            return Ok(result);
        }
        [HttpPost("/reports/funnel-report")]
        public async Task<IActionResult> FunnelReport([FromBody] DateRangeDto Date)
        {
            var result = await _reportsService.ReportFunnel(Date.DateStart, Date.DateEnd);
            return Ok(result);
        }

    }
}
