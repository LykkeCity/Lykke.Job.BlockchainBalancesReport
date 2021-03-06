﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class FileSystemReportRepository : IReportRepository
    {
        private readonly ILog _logger;
        private readonly string _filePath;
        private readonly ConcurrentBag<ReportItem> _items;

        public FileSystemReportRepository(
            ILogFactory logFactory,
            ReportFileRepositorySettings settings)
        {
            _logger = logFactory.CreateLog(this);
            _filePath = settings.FilePath;

            _items = new ConcurrentBag<ReportItem>();
        }

        public Task AddBalanceAsync(ReportItem item)
        {
            _items.Add(item);

            return Task.CompletedTask;
        }

        public async Task FlushAsync()
        {
            foreach (var group in _items.GroupBy(x => x.At))
            {
                await SaveReportFileAsync(group.Key, group.ToArray());
            }
        }

        private async Task SaveReportFileAsync(DateTime at, IReadOnlyCollection<ReportItem> items)
        {
            var filePath = _filePath.Replace("{datetime}", at.ToString("yyyy-MM-ddTHH-mm-ss"));

            _logger.Info($"Saving balances report to {filePath}...");

            var stream = File.Open
            (
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read
            );
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await writer.WriteLineAsync
                    ("date (UTC),blockchain,addressName,address,asset,balance,blockchain asset ID,asset ID,explorer");

                foreach (var i in items.OrderBy(x => x.BlockchainType).ThenBy(x => x.AddressName))
                {
                    await writer.WriteAsync($"{at:yyyy-MM-ddTHH:mm:ss},");
                    await writer.WriteAsync($"{i.BlockchainType},");
                    await writer.WriteAsync($"{i.AddressName},");
                    await writer.WriteAsync($"{i.Address},");
                    await writer.WriteAsync($"{i.Asset.Name},");
                    await writer.WriteAsync($"{i.Balance.ToString(CultureInfo.InvariantCulture)},");
                    await writer.WriteAsync($"{i.Asset.BlockchainId},");
                    await writer.WriteAsync($"{i.Asset.LykkeId},");
                    await writer.WriteLineAsync(i.ExplorerUrl);
                }
            }

            _logger.Info($"Balances report saving done. {items.Count} balances saved");
        }
    }
}
