using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class ReportRepositoriesSettings
    {
        [Optional]
        public ReportFileRepositorySettings File { get; set; }
        
        [Optional]
        public AzureSqlRepositorySettings Sql { get; set; }
    }
}
