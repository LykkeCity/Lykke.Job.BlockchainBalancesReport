using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.EosParkApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Eos
{
    public class EosBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Eos";

        private readonly ILogger<EosBalanceProvider> _logger;
        private readonly EosParkApiClient _client;

        public EosBalanceProvider(
            ILogger<EosBalanceProvider> logger,
            IOptions<EosSettings> settings)
        {
            _logger = logger;
            _client = new EosParkApiClient(settings.Value.ParkApiUrl, settings.Value.ApiKey);
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var page = 1;
            var balances = new Dictionary<string, decimal>();
            var transactionsRead = 0;

            do
            {
                var response = await Policy
                    .Handle<Exception>(ex =>
                    {
                        _logger.LogWarning(ex, $"Failed to get transactions page {page} of {address}. Operation will be retried.");
                        return true;
                    })
                    .OrResult<EosParkApiAccountTransactionsResponse>(x =>
                    {
                        if (x.ErrNo != 0)
                        {
                            _logger.LogWarning($"Failed to get transactions page {page} of {address}: {x.ErrNo} - {x.ErrMsg}. Operation will be retried.");
                            return true;
                        }
                        return false;
                    })
                    .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(Math.Min(i, 5)))
                    .ExecuteAsync(async () => await _client.GetAccountTransactions(address, page));

                foreach (var tx in response.Data.TraceList)
                {
                    ++transactionsRead;

                    if (tx.Status != "executed")
                    {
                        throw new NotSupportedException($"Only executed transactions are supported. Implement support of new status - {tx.Status}");
                    }

                    if (tx.Timestamp > at)
                    {
                        continue;
                    }

                    var txAmount = decimal.Parse(tx.Quantity, CultureInfo.InvariantCulture);
                    var balanceChange = address.Equals(tx.Receiver, StringComparison.InvariantCultureIgnoreCase)
                        ? txAmount
                        : -txAmount;

                    if (!balances.TryGetValue(tx.Symbol, out var balance))
                    {
                        balances.Add(tx.Symbol, balanceChange);
                    }
                    else
                    {
                        balances[tx.Symbol] = balance + balanceChange;
                    }
                }

                if (transactionsRead >= response.Data.TraceCount)
                {
                    break;
                }

                ++page;
            }
            while (true);

            return balances.ToDictionary(x => GetBalancesKey(x.Key), x => x.Value);
        }

        private static (string BlockchainAsset, string AssetId) GetBalancesKey(string symbol)
        {
            return symbol == "EOS" 
                ? (symbol, "782e7e92-2ce0-4b21-b425-6096983351af") 
                : (symbol, null);
        }
    }
}
