using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public class BalancesReport
    {
        private readonly ILogger<BalancesReport> _logger;
        private readonly IOptions<ReportSettings> _settings;
        private readonly List<ReportItem> _items;
        private bool _saved;

        public BalancesReport(
            ILogger<BalancesReport> logger,
            IOptions<ReportSettings> settings)
        {
            _logger = logger;
            _settings = settings;
            _items = new List<ReportItem>();
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

            var filePath = _settings.Value.ReportFilePath;
            var date = _settings.Value.BalancesAt;

            _logger.LogInformation($"Saving balances report to {filePath}...");

            var stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await writer.WriteLineAsync("date (UTC),blockchain,addressName,address,blockchain asset,asset ID,balance,explorer");

                foreach (var i in _items)
                {
                    await writer.WriteAsync($"{date:yyyy-MM-ddTHH:mm:ss},");
                    await writer.WriteAsync($"{i.BlockchainType},");
                    await writer.WriteAsync($"{i.AddressName},");
                    await writer.WriteAsync($"{i.Address},");
                    await writer.WriteAsync($"{i.BlockchainAsset},");
                    await writer.WriteAsync($"{i.AssetId},");
                    await writer.WriteAsync($"{i.Balance.ToString(CultureInfo.InvariantCulture)},");
                    await writer.WriteLineAsync(i.ExplorerUrl);
                }
            }

            _logger.LogInformation($"Balances report saving done. {_items.Count} balances saved");
        }
    }
}
