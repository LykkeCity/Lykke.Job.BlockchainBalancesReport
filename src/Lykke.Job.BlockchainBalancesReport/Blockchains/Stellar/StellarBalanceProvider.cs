using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.Horizon;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Options;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Stellar
{
    public class StellarBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Stellar";

        private readonly HorizonBalanceProvider _horizonBalanceProvider;

        public StellarBalanceProvider(IOptions<StellarSettings> settings)
        {
            _horizonBalanceProvider = new HorizonBalanceProvider
            (
                settings.Value.HorizonUrl,
                nativeAssetMultiplier: 0.0000001M,
                nativeAssetCode: "XLM"
            );
        }

        public async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balances = await _horizonBalanceProvider.GetBalancesAsync(address, at);

            return balances.ToDictionary(x => GetBalancesKey(x.Key), x => x.Value);
        }

        private static Asset GetBalancesKey(string assetType)
        {
            return assetType == "XLM"
                ? new Asset("XLM", "XLM", "b5a0389c-fe57-425f-ab17-af41638f6b89")
                : new Asset(assetType, assetType, null);
        }
    }
}
