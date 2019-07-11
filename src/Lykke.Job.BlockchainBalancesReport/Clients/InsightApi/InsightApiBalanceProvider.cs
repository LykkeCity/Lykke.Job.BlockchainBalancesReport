using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiBalanceProvider
    {
        private readonly ILogger<InsightApiBalanceProvider> _logger;
        private readonly InsightApiClient _insightApiClient;
        private readonly Func<string, string> _addressNormalizer;

        public InsightApiBalanceProvider(
            ILogger<InsightApiBalanceProvider> logger,
            InsightApiClient insightApiClient,
            Func<string, string> addressNormalizer)
        {
            _logger = logger;
            _insightApiClient = insightApiClient;
            _addressNormalizer = addressNormalizer;
        }

        public async Task<decimal> GetBalanceAsync(string address, DateTime at)
        {
            decimal balance = 0;
            var page = 0;
            var atTime = new DateTimeOffset(at).ToUnixTimeSeconds();
            var normalizedAddress = _addressNormalizer.Invoke(address);

            if (normalizedAddress == null)
            {
                throw new InvalidOperationException($"Invalid address: {address}");
            }

            do
            {
                var response = await Policy
                    .Handle<Exception>(ex =>
                    {
                        _logger.LogWarning(ex, $"Failed to get transactions page {page} of {address}. Operation will be retried.");
                        return true;
                    })
                    .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(Math.Min(i, 5)))
                    .ExecuteAsync(async () => await _insightApiClient.GetAddressTransactions(normalizedAddress, page));

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

        public async Task<decimal> GetBalanceAsync2(string address, DateTime at)
        {
            decimal balance = 0;
            var from = 0;
            var atTime = new DateTimeOffset(at).ToUnixTimeSeconds();
            var normalizedAddress = _addressNormalizer.Invoke(address);

            if (normalizedAddress == null)
            {
                throw new InvalidOperationException($"Invalid address: {address}");
            }

            do
            {
                var response = await Policy
                    .Handle<Exception>(ex =>
                    {
                        _logger.LogWarning(ex, $"Failed to get transactions page {from} of {address}. Operation will be retried.");
                        return true;
                    })
                    .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(Math.Min(i, 5)))
                    .ExecuteAsync(async () => await _insightApiClient.GetAddressTransactions2(normalizedAddress, from));

                var sum = response.Transactions
                    .Where(x => x.Time <= atTime)
                    .Select(x => GetTransactionValue(x, normalizedAddress))
                    .Sum();

                balance += sum;

                if (!response.Transactions.Any())
                {
                    break;
                }

                from = response.To;

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
