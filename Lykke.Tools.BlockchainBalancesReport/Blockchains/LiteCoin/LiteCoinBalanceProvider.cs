using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Altcoins;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.LiteCoin
{
    public class LiteCoinBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "LiteCoin";

        private readonly Network _network;
        private readonly InsightApiBalanceProvider _balanceProvider;

        public LiteCoinBalanceProvider(
            ILoggerFactory loggerFactory,
            IOptions<LiteCoinSettings> settings)
        {
            Litecoin.Instance.EnsureRegistered();

            _network = Litecoin.Instance.Mainnet;
            _balanceProvider = new InsightApiBalanceProvider
            (
                loggerFactory.CreateLogger<InsightApiBalanceProvider>(),
                new InsightApiClient(settings.Value.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(
            string address,
            DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<(string BlockchainAsset, string AssetId), decimal>
            {
                {("LTC", "2971fbd8-8cbc-4797-8823-9fbde8be3b1c"), balance}
            };
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
