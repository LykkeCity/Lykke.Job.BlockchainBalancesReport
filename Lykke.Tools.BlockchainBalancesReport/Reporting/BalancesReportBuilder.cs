using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Blockchains;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public class BalancesReportBuilder
    {
        private readonly ILogger<BalancesReportBuilder> _logger;
        private readonly IOptions<ReportSettings> _reportSettings;
        private readonly BalanceProvidersFactory _balanceProvidersFactory;
        private readonly ExplorerUrlFormattersFactory _explorerUrlFormattersFactory;
        private readonly BalancesReport _report;

        public BalancesReportBuilder(
            ILogger<BalancesReportBuilder> logger,
            IOptions<ReportSettings> reportSettings,
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

        public async Task BuildAsync()
        {
            var settings = _reportSettings.Value;
            
            _logger.LogInformation($"Building balances report at {settings.BalancesAt:yyyy-MM-ddTHH:mm:ss} UTC...");
            
            var tasks = new List<Task>();

            foreach (var (blockchainType, namedAddresses) in settings.Addresses)
            {
                tasks.Add(BuildBlockchainReportAsync(blockchainType, namedAddresses, settings));
            }

            await Task.WhenAll(tasks);

            _logger.LogInformation("Balances report building done");

            await _report.SaveAsync();
        }

        private async Task BuildBlockchainReportAsync(
            string blockchainType, 
            IReadOnlyDictionary<string, string> namedAddresses,
            ReportSettings settings)
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
                    .ExecuteAsync(async () => await balanceProvider.GetBalancesAsync(address, settings.BalancesAt));
                
                foreach (var ((blockchainAsset, assetId), balance) in assetBalances)
                {
                    var explorerUrl = explorerUrlFormatter?.Format(address, blockchainAsset);

                    _report.AddBalance
                    (
                        blockchainType,
                        addressName,
                        address,
                        blockchainAsset,
                        assetId,
                        balance,
                        explorerUrl
                    );
                }
            }
        }
    }
}
