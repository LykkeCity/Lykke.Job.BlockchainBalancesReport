using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiBalanceProvider
    {
        private readonly InsightApiClient _insightApiClient;
        private readonly Func<string, string> _addressNormalizer;

        public InsightApiBalanceProvider(
            InsightApiClient insightApiClient,
            Func<string, string> addressNormalizer)
        {
            _insightApiClient = insightApiClient;
            _addressNormalizer = addressNormalizer;
        }

        public async Task<decimal> GetBalanceAsync(string address, DateTime at)
        {
            decimal balance = 0;
            var page = 0;
            var atTime = new DateTimeOffset(at, TimeSpan.Zero).ToUnixTimeSeconds();
            var normalizedAddress = _addressNormalizer.Invoke(address);

            if (normalizedAddress == null)
            {
                throw new InvalidOperationException($"Invalid address: {address}");
            }

            do
            {
                var response = await _insightApiClient.GetAddressTransactions(normalizedAddress, page);

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

            return balance;
        }

        private decimal GetTransactionValue(InsightApiTransaction tx, string forAddress)
        {
            var inputs = tx.Inputs
                .Where(i => _addressNormalizer.Invoke(i.Address) == forAddress)
                .Sum(i => i.Value);
            var outputs = tx.Outputs
                .Where(o => o.ScriptPubKey.Addresses != null &&
                            o.ScriptPubKey.Addresses
                                .Select(_addressNormalizer)
                                .Contains(forAddress))
                .Sum(o => decimal.Parse(o.Value, CultureInfo.InvariantCulture));

            return outputs - inputs;
        }
    }
}
