﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Microsoft.Extensions.Options;

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
                        logFactory,
                        s.Repositories.File
                    )
                );
            }

            if (s.Repositories.Sql != null)
            {
                repositories.Add
                (
                    new AzureSqlReportRepository
                    (
                        logFactory,
                        s.Repositories.Sql
                    )
                );
            }

            _date = s.BalancesAt;
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