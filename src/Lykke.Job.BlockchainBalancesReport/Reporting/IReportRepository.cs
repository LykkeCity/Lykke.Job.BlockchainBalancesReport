using System.Threading.Tasks;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public interface IReportRepository
    {
        Task AddBalanceAsync(ReportItem item);
        Task FlushAsync();
    }
}
