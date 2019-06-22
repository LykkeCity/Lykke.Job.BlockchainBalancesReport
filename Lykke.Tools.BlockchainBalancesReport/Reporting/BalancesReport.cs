using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public class BalancesReport
    {
        private readonly List<ReportItem> _items;
        private bool _saved;
        private readonly IReadOnlyCollection<IReportRepository> _reportRepositories;

        public BalancesReport(
            ILoggerFactory loggerFactory,
            IOptions<ReportSettings> settings)
        {
            _items = new List<ReportItem>();

            var s = settings.Value;
            var repositories = new List<IReportRepository>();
                
            if (s.Repositories.File != null)
            {
                repositories.Add
                (
                    new FileSystemReportRepository
                    (
                        loggerFactory.CreateLogger<FileSystemReportRepository>(),
                        s.Repositories.File
                    )
                );
            }

            if (s.Repositories.Sql != null)
            {
                repositories.Add(new AzureSqlReportRepository(TODO, s.Repositories.Sql));
            }

            _reportRepositories = repositories;
        }

        public void AddBalance(
            string blockchainType, 
            string addressName, 
            string address, 
            string blockchainAsset,
            string assetId,
            decimal balance, 
            string explorerUrl)
        {
            if (_saved)
            {
                throw new InvalidOperationException("Report already saved");
            }

            _items.Add(new ReportItem
            {
                BlockchainType = blockchainType,
                AddressName = addressName,
                Address = address,
                BlockchainAsset = blockchainAsset,
                AssetId = assetId,
                Balance = balance,
                ExplorerUrl = explorerUrl
            });
        }

        public async Task SaveAsync()
        {
            _saved = true;

            var tasks = _reportRepositories.Select(x => x.SaveAsync(_items));

            await Task.WhenAll(tasks);
        }
    }
}
