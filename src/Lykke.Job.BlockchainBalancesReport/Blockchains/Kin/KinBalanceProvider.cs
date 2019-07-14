using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Clients.Horizon;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Kin
{
    public class KinBalanceProvider : IBalanceProvider
    {
        public Task AsyncInitialization => Task.CompletedTask;
        public string BlockchainType => "Kin";

        private readonly HorizonBalanceProvider _horizonBalanceProvider;

        public KinBalanceProvider(KinSettings settings)
        {
            _horizonBalanceProvider = new HorizonBalanceProvider
            (
                settings.HorizonUrl,
                nativeAssetMultiplier: 0.00001M,
                nativeAssetCode: "KIN"
            );
        }

        public async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balances = await _horizonBalanceProvider.GetBalancesAsync(address, at);

            return balances.ToDictionary(x => GetBalancesKey(x.Key), x => x.Value);
        }

        private BlockchainAsset GetBalancesKey(string assetType)
        {
            return assetType == "KIN"
                ? new BlockchainAsset("KIN", "KIN", "568637d4-2b03-4f66-972e-b947a40f2771")
                : new BlockchainAsset(assetType, assetType, null);
        }
    }
}
