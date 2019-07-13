using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Polly;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class BalancesReportBuilder
    {
        private readonly ILog _log;
        private readonly ReportSettings _reportSettings;
        private readonly BalanceProvidersFactory _balanceProvidersFactory;
        private readonly ExplorerUrlFormattersFactory _explorerUrlFormattersFactory;
        private readonly Func<BalancesReport> _reportFactory;
        
        public BalancesReportBuilder(
            ILogFactory logFactory,
            ReportSettings reportSettings,
            BalanceProvidersFactory balanceProvidersFactory,
            ExplorerUrlFormattersFactory explorerUrlFormattersFactory,
            Func<BalancesReport> reportFactoryFactory)
        {
            _log = logFactory.CreateLog(this);
            _reportSettings = reportSettings;
            _balanceProvidersFactory = balanceProvidersFactory;
            _explorerUrlFormattersFactory = explorerUrlFormattersFactory;
            _reportFactory = reportFactoryFactory;
        }

        public async Task BuildAsync(DateTime at)
        {
            _log.Info($"Building balances report at {at:yyyy-MM-ddTHH:mm:ss} UTC...");

            var report = _reportFactory.Invoke();
            var tasks = new List<Task>();

            foreach (var (blockchainType, namedAddresses) in _reportSettings.Addresses)
            {
                tasks.Add(BuildBlockchainReportAsync(report, blockchainType, namedAddresses, at));
            }

            await Task.WhenAll(tasks);

            _log.Info("Balances report building done");

            await report.SaveAsync(at);
        }

        private async Task BuildBlockchainReportAsync(
            BalancesReport report, 
            string blockchainType,
            IReadOnlyDictionary<string, string> namedAddresses,
            DateTime at)
        {
            var balanceProvider = _balanceProvidersFactory.GetBalanceProvider(blockchainType);
            var explorerUrlFormatter = _explorerUrlFormattersFactory.GetFormatterOrDefault(blockchainType);

            foreach (var (addressName, address) in namedAddresses)
            {
                _log.Info($"Getting balances of {blockchainType} {addressName}: {address}...");

                var assetBalances = await Policy
                    .Handle<Exception>(ex =>
                    {
                        _log.Warning($"Failed to get balances of {blockchainType}:{addressName}. Operation will be retried.", ex);
                        return true;
                    })
                    .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(Math.Min(i, 5)))
                    .ExecuteAsync(async () => await balanceProvider.GetBalancesAsync(address, at));
                
                foreach (var (asset, balance) in assetBalances)
                {
                    var explorerUrl = explorerUrlFormatter?.Format(address, asset);

                    report.AddBalance
                    (
                        blockchainType,
                        addressName,
                        address,
                        asset,
                        balance,
                        explorerUrl
                    );
                }
            }
        }
    }
}
