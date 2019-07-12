using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Microsoft.Extensions.Logging;
using Polly;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class BalancesReportBuilder
    {
        private readonly ILogger<BalancesReportBuilder> _logger;
        private readonly ReportSettings _reportSettings;
        private readonly BalanceProvidersFactory _balanceProvidersFactory;
        private readonly ExplorerUrlFormattersFactory _explorerUrlFormattersFactory;
        private readonly BalancesReport _report;

        public BalancesReportBuilder(
            ILogger<BalancesReportBuilder> logger,
            ReportSettings reportSettings,
            BalanceProvidersFactory balanceProvidersFactory,
            ExplorerUrlFormattersFactory explorerUrlFormattersFactory,
            BalancesReport report)
        {
            _logger = logger;
            _reportSettings = reportSettings;
            _balanceProvidersFactory = balanceProvidersFactory;
            _explorerUrlFormattersFactory = explorerUrlFormattersFactory;
            _report = report;
        }

        public async Task BuildAsync(DateTime at)
        {
            _logger.LogInformation($"Building balances report at {at:yyyy-MM-ddTHH:mm:ss} UTC...");
            
            var tasks = new List<Task>();

            foreach (var (blockchainType, namedAddresses) in _reportSettings.Addresses)
            {
                tasks.Add(BuildBlockchainReportAsync(blockchainType, namedAddresses, at));
            }

            await Task.WhenAll(tasks);

            _logger.LogInformation("Balances report building done");

            await _report.SaveAsync();
        }

        private async Task BuildBlockchainReportAsync(
            string blockchainType, 
            IReadOnlyDictionary<string, string> namedAddresses,
            DateTime at)
        {
            var balanceProvider = _balanceProvidersFactory.GetBalanceProvider(blockchainType);
            var explorerUrlFormatter = _explorerUrlFormattersFactory.GetFormatterOrDefault(blockchainType);

            foreach (var (addressName, address) in namedAddresses)
            {
                _logger.LogInformation($"Getting balances of {blockchainType} {addressName}: {address}...");

                var assetBalances = await Policy
                    .Handle<Exception>(ex =>
                    {
                        _logger.LogWarning(ex, $"Failed to get balances of {blockchainType}:{addressName}. Operation will be retried.");
                        return true;
                    })
                    .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(Math.Min(i, 5)))
                    .ExecuteAsync(async () => await balanceProvider.GetBalancesAsync(address, at));
                
                foreach (var (asset, balance) in assetBalances)
                {
                    var explorerUrl = explorerUrlFormatter?.Format(address, asset);

                    _report.AddBalance
                    (
                        blockchainType,
                        addressName,
                        address,
                        asset,
                        balance,
                        explorerUrl,
                        at
                    );
                }
            }
        }
    }
}
