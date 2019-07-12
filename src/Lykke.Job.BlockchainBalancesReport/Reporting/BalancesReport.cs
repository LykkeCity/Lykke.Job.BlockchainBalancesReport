﻿using System;
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
        private readonly List<ReportItem> _items;
        private bool _saved;
        private readonly IReadOnlyCollection<IReportRepository> _reportRepositories;
        private readonly DateTime _date;

        public BalancesReport(
            ILogFactory logFactory,
            ReportSettings settings)
        {
            _items = new List<ReportItem>();

            var repositories = new List<IReportRepository>();
                
            if (settings.Repositories.File != null)
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

            if (settings.Repositories.Sql != null)
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

            _date = settings.BalancesAt;
            _reportRepositories = repositories;
        }

        public void AddBalance(
            string blockchainType, 
            string addressName, 
            string address, 
            Asset asset,
            decimal balance, 
            string explorerUrl)
        {
            if (_saved)
            {
                throw new InvalidOperationException("Report already saved");
            }

            _items.Add(new ReportItem
            {
                Date = _date,
                BlockchainType = blockchainType,
                AddressName = addressName,
                Address = address,
                Asset = asset,
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
