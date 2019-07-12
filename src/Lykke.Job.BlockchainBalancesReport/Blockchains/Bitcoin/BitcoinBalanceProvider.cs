using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Clients.Ninja;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Bitcoin
{
    public class BitcoinBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Bitcoin";

        private readonly NinjaClient _client;

        public BitcoinBalanceProvider(BitcoinSettings settings)
        {
            _client = new NinjaClient(settings.NinjaUrl);
        }

        public async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address,
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

            return new Dictionary<Asset, decimal>
            {
                {new Asset("BTC", "BTC", "BTC"), balance}
            };
        }
    }
}
