using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public interface IReportRepository
    {
        Task SaveAsync(IReadOnlyCollection<ReportItem> items);
    }
}