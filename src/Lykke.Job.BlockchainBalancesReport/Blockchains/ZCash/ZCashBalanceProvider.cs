using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.ZCash
{
    public class ZCashBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "ZCash";

        private readonly InsightApiBalanceProvider _balanceProvider;

        public ZCashBalanceProvider(
            ILogFactory logFactory,
            ZCashSettings settings)
        {
            _balanceProvider = new InsightApiBalanceProvider
            (
                logFactory,
                new InsightApiClient(settings.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<BlockchainAsset, decimal>
            {
                {new BlockchainAsset("ZEC", "ZEC", "b2c591a2-6c2d-4130-89cd-71813481bb76"), balance}
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
