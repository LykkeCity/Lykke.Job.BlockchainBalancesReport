using System.Runtime.InteropServices;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class ScheduleSettings
    {
        public bool IsEnabled { get; set; }
        public string BuildReportCron { get; set; }
    }
}
