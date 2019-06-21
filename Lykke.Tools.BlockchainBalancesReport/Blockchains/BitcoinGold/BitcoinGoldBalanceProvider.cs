using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Altcoins;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.BitcoinGold
{
    public class BitcoinGoldBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "BitcoinGold";

        private readonly InsightApiClient _client;
        private readonly Network _network;

        public BitcoinGoldBalanceProvider(IOptions<BitcoinGoldSettings> settings)
        {
            BGold.Instance.EnsureRegistered();

            _client = new InsightApiClient(settings.Value.InsightApiUrl);
            _network = BGold.Instance.Mainnet;
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(string address, DateTime at)
        {
            decimal balance = 0;
            var page = 0;
            var atTime = new DateTimeOffset(at, TimeSpan.Zero).ToUnixTimeSeconds();
            var normalizedAddress = NormalizeOrDefault(address);

            if (normalizedAddress == null)
            {
                throw new InvalidOperationException($"Invalid BCH address: {address}");
            }

            do
            {
                var response = await _client.GetAddressTransactions(normalizedAddress, page);

                var sum = response.Transactions
                    .Where(x => x.Time <= atTime)
                    .Select(x => GetTransactionValue(x, normalizedAddress))
                    .Sum();

                balance += sum;

                if (++page >= response.PagesTotal)
                {
                    break;
                }

            } while (true);

            return new Dictionary<(string BlockchainAsset, string AssetId), decimal>
            {
                {("BTG", "a4954205-48eb-4286-9c82-07792169f4db"), balance}
            };
        }

        private decimal GetTransactionValue(InsightApiTransaction tx, string forAddress)
        {
            var inputs = tx.Inputs
                .Where(i => NormalizeOrDefault(i.Address) == forAddress)
                .Sum(i => i.Value);
            var outputs = tx.Outputs
                .Where(o => o.ScriptPubKey.Addresses != null &&
                            o.ScriptPubKey.Addresses
                                .Select(NormalizeOrDefault)
                                .Contains(forAddress))
                .Sum(o => decimal.Parse(o.Value, CultureInfo.InvariantCulture));

            return outputs - inputs;
        }

        private string NormalizeOrDefault(string address)
        {
            try
            {
                var bitcoinAddress = BitcoinAddress.Create(address, _network);

                return bitcoinAddress.ToString();
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
