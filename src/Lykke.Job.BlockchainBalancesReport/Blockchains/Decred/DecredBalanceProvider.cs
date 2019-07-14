using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Decred
{
    public class DecredBalanceProvider : IBalanceProvider
    {
        public Task AsyncInitialization => Task.CompletedTask;
        public string BlockchainType => "Decred";

        private readonly InsightApiBalanceProvider _balanceProvider;
        
        public DecredBalanceProvider(
            ILogFactory logFactory,
            DecredSettings settings) : 
            
            this(logFactory, settings.InsightApiUrl)
        {
        }

        public DecredBalanceProvider(
            ILogFactory logFactory,
            string insightApiUrl)
        {
            _balanceProvider = new InsightApiBalanceProvider
            (
                logFactory,
                new InsightApiClient(insightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address,
            DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync2(address, at);

            return new Dictionary<BlockchainAsset, decimal>
            {
                {new BlockchainAsset("DCR", "DCR", "02154b48-7ed9-4211-b614-e87679fd4f5a"), balance}
            };
        }

        private string NormalizeOrDefault(string address)
        {
            return address;
        }
    }
}
