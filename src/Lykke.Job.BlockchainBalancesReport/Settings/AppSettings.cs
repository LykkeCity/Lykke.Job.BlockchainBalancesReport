using Lykke.Job.BlockchainBalancesReport.Settings.JobSettings;
using Lykke.Sdk.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public BlockchainBalancesReportJobSettings BlockchainBalancesReportJob { get; set; }
    }
}
