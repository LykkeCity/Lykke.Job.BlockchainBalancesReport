using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.ZCash
{
    public class ZCashBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "ZCash";

        private readonly InsightApiBalanceProvider _balanceProvider;

        public ZCashBalanceProvider(
            ILoggerFactory loggerFactory,
            IOptions<ZCashSettings> settings)
        {
            _balanceProvider = new InsightApiBalanceProvider
            (
                loggerFactory.CreateLogger<InsightApiBalanceProvider>(),
                new InsightApiClient(settings.Value.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<(string BlockchainAsset, string AssetId), decimal>
            {
                {("ZEC", "b2c591a2-6c2d-4130-89cd-71813481bb76"), balance}
            };
        }

        private static string NormalizeOrDefault(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                return address;
            }

            return null;
        }
    }
}
