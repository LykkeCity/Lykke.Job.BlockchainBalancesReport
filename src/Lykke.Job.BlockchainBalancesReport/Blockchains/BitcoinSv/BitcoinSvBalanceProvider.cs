using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Job.BlockchainBalancesReport.Settings;
using NBitcoin;
using NBitcoin.Altcoins;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.BitcoinSv
{
    public class BitcoinSvBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "BitcoinSv";

        private readonly Network _network;
        private readonly InsightApiBalanceProvider _balanceProvider;

        public BitcoinSvBalanceProvider(
            ILogFactory logFactory,
            BitcoinSvSettings settings)
        {
            BCash.Instance.EnsureRegistered();

            _network = Network.Main;
            _balanceProvider = new InsightApiBalanceProvider
            (
                logFactory,
                new InsightApiClient(settings.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address,
            DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<BlockchainAsset, decimal>
            {
                {new BlockchainAsset("BSV", "BSV", "d027469e-04a1-4578-8e4f-a7e0e9e40132"), balance}
            };
        }
        
        private string NormalizeOrDefault(string address)
        {
            try
            {
                return BitcoinAddress.Create(address, _network)?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
