using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public interface IReportRepository
    {
        Task SaveAsync(DateTime at, IReadOnlyCollection<ReportItem> items);
    }
}
