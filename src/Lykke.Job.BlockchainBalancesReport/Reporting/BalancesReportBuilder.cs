using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Lykke.Job.BlockchainBalancesReport.Utils;
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

            _log.Info("Flushing balance report report");

            await report.FlushAsync();

            _log.Info("Balances report flushing done");
        }

        private async Task BuildBlockchainReportAsync(
            BalancesReport report, 
            string blockchainType,
            IReadOnlyDictionary<string, string> namedAddresses,
            DateTime at)
        {
            var balanceProvider = _balanceProvidersFactory.GetBalanceProvider(blockchainType);
            var explorerUrlFormatter = _explorerUrlFormattersFactory.GetFormatterOrDefault(blockchainType);

            if(balanceProvider is IAsyncInitialization initialization)
            {
                await initialization.AsyncInitialization;
            }

            foreach (var (addressName, address) in namedAddresses)
            {
                _log.Info($"Getting balances of {blockchainType} {addressName}: {address}...");

                try
                {
                    var assetBalances = await Policy
                        .Handle<Exception>
                        (
                            ex =>
                            {
                                _log.Warning
                                (
                                    $"Failed to get balances of {blockchainType}:{addressName}. Operation will be retried.",
                                    ex
                                );
                                return true;
                            }
                        )
                        .WaitAndRetryAsync(20, i => TimeSpan.FromSeconds(Math.Min(i, 5)))
                        .ExecuteAsync(async () => await balanceProvider.GetBalancesAsync(address, at));

                    foreach (var (asset, balance) in assetBalances)
                    {
                        try
                        {
                            var explorerUrl = explorerUrlFormatter?.Format(address, asset);

                            await report.AddBalanceAsync
                            (
                                at,
                                blockchainType,
                                addressName,
                                address,
                                asset,
                                balance,
                                explorerUrl
                            );
                        }
                        catch (Exception ex)
                        {
                            _log.Warning($"Failed to add balance to the report {blockchainType}:{addressName}:{asset}={balance}", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Warning($"Failed to process {blockchainType}:{addressName}", ex);
                }
            }
        }
    }
}
