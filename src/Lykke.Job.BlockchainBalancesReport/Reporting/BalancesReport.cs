using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class BalancesReport
    {
        private bool _flushed;
        private readonly IReadOnlyCollection<IReportRepository> _reportRepositories;

        public BalancesReport(
            ILogFactory logFactory,
            ReportSettings settings)
        {
            var repositories = new List<IReportRepository>();
                
            if (settings.Repositories.File.IsEnabled)
            {
                repositories.Add
                (
                    new FileSystemReportRepository
                    (
                        logFactory,
                        settings.Repositories.File
                    )
                );
            }

            if (settings.Repositories.Sql.IsEnabled)
            {
                repositories.Add
                (
                    new AzureSqlReportRepository
                    (
                        logFactory,
                        settings.Repositories.Sql
                    )
                );
            }

            _reportRepositories = repositories;
        }

        public async Task AddBalanceAsync(
            DateTime at,
            string blockchainType, 
            string addressName, 
            string address, 
            BlockchainAsset asset,
            decimal balance, 
            string explorerUrl)
        {
            if (_flushed)
            {
                throw new InvalidOperationException("Report already flushed");
            }

            var item = new ReportItem
            {
                At = at,
                BlockchainType = blockchainType,
                AddressName = addressName,
                Address = address,
                Asset = asset,
                Balance = balance,
                ExplorerUrl = explorerUrl
            };

            var tasks = _reportRepositories.Select(x => x.AddBalanceAsync(item));

            await Task.WhenAll(tasks);
        }

        public async Task FlushAsync()
        {
            if (_flushed)
            {
                return;
            }

            _flushed = true;

            var tasks = _reportRepositories.Select(x => x.FlushAsync());

            await Task.WhenAll(tasks);
        }
    }
}
