using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains
{
    public class BalanceProvidersFactory
    {
        private readonly IReadOnlyDictionary<string, IBalanceProvider> _balanceProviders;

        public BalanceProvidersFactory(IEnumerable<IBalanceProvider> balanceProviders)
        {
            _balanceProviders = balanceProviders.ToDictionary(x => x.BlockchainType);
        }

        public IBalanceProvider GetBalanceProvider(string blockchainType)
        {
            if (_balanceProviders.TryGetValue(blockchainType, out var balanceProvider))
            {
                return balanceProvider;
            }

            throw new InvalidOperationException($"Balance provider for blockchain {blockchainType} not found");
        }
    }
}
