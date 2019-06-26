using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.Horizon;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Options;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Kin
{
    public class KinBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Kin";

        private readonly HorizonBalanceProvider _horizonBalanceProvider;

        public KinBalanceProvider(IOptions<KinSettings> settings)
        {
            _horizonBalanceProvider = new HorizonBalanceProvider
            (
                settings.Value.HorizonUrl,
                nativeAssetMultiplier: 0.00001M,
                nativeAssetCode: "KIN"
            );
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balances = await _horizonBalanceProvider.GetBalancesAsync(address, at);

            return balances.ToDictionary(x => GetBalancesKey(x.Key), x => x.Value);
        }

        private static (string BlockchainAsset, string AssetId) GetBalancesKey(string assetType)
        {
            return assetType == "KIN"
                ? ("KIN", "568637d4-2b03-4f66-972e-b947a40f2771")
                : (assetType, null);
        }
    }
}
