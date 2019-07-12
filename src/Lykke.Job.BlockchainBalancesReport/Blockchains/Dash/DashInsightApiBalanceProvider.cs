using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Microsoft.Extensions.Options;
using NBitcoin;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Dash
{
    public class DashInsightApiBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Dash";

        private readonly Network _network;
        private readonly InsightApiBalanceProvider _balanceProvider;

        public DashInsightApiBalanceProvider(
            ILogFactory logFactory, 
            IOptions<DashSettings> settings)
        {
            NBitcoin.Altcoins.Dash.Instance.EnsureRegistered();

            _network = NBitcoin.Altcoins.Dash.Instance.Mainnet;
            _balanceProvider = new InsightApiBalanceProvider
            (
                logFactory,
                new InsightApiClient(settings.Value.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<Asset, decimal>
            {
                {new Asset("DASH", "DASH", "4d498e43-956f-45ee-be07-8bb435003f26"), balance}
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
