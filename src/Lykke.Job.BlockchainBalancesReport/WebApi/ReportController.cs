using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Reporting;
using Lykke.Job.BlockchainBalancesReport.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Job.BlockchainBalancesReport.WebApi
{
    [Route("api/report")]
    public class ReportController : Controller
    {
        private readonly BalancesReportBuilder _reportBuilder;

        public ReportController(BalancesReportBuilder reportBuilder)
        {
            _reportBuilder = reportBuilder;
        }

        [HttpPost("build")]    
        public async Task<IActionResult> BuildReport(BuildReportRequest request)
        {
            await _reportBuilder.BuildAsync(request.At.UtcDateTime);

            return Ok();
        }
    }
}
