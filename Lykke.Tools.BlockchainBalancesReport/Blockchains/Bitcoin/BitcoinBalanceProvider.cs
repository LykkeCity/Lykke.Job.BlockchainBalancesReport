using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.Ninja;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Options;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Bitcoin
{
    public class BitcoinBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Bitcoin";

        private readonly NinjaClient _client;

        public BitcoinBalanceProvider(IOptions<BitcoinSettings> settings)
        {
            _client = new NinjaClient(settings.Value.NinjaUrl);
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(
            string address, 
            DateTime at)
        {
            string continuation = null;
            long satoshiBalance = 0;

            do
            {
                var response = await _client.GetBalancesAsync(address, false, continuation);

                satoshiBalance += response.Operations
                    .Where(x => x.FirstSeen <= at)
                    .Sum(x => x.Amount);

                continuation = response.Continuation;

            } while (continuation != null);

            var balance = satoshiBalance * 0.00000001M;

            return new Dictionary<(string BlockchainAsset, string AssetId), decimal>
            {
                {(BlockchainAsset: "BTC", AssetId: "BTC"), balance}
            };
        }
    }
}
