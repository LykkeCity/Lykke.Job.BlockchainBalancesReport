using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class ReportAzureSqlRepositorySettings
    {
        [Optional]
        public bool IsEnabled { get; set; }
        
        [Optional]
        public string ConnString { get; set; }

        [Optional]
        public bool CreateTable { get; set; }
    }
}
