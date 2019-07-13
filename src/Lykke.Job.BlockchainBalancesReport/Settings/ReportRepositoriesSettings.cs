using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class ReportRepositoriesSettings
    {
        public ReportFileRepositorySettings File { get; set; }
        
        public ReportAzureSqlRepositorySettings Sql { get; set; }
    }
}
