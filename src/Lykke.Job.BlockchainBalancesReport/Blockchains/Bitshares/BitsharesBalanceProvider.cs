using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Job.BlockchainBalancesReport.Clients.BitsharesExplorer;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Bitshares
{
    public class BitsharesBalanceProvider : IBalanceProvider
    {
        public Task AsyncInitialization => Task.CompletedTask;
        public string BlockchainType => "Bitshares";

        private readonly string _baseUrl;
        private readonly Dictionary<string, (BlockchainAsset asset, int precision)> _cachedAssets;

        private readonly Dictionary<string, (string assetName, string lykkeAssetId)> _predefinedAssets;

        // ReSharper disable once UnusedMember.Global
        public BitsharesBalanceProvider(BitsharesSettings settings) :
            this(settings.ExplorerBaseUrl)
        {
        }

        public BitsharesBalanceProvider(string baseUrl)
        {
            _baseUrl = baseUrl;
            
            _predefinedAssets = new Dictionary<string, (string assetName, string lykkeAssetId)>
            {
                {"1.3.0", (assetName: "BTS", lykkeAssetId:"20ce0468-917e-4097-abba-edf7c8600cfb")}
            };

            _cachedAssets = new Dictionary<string, (BlockchainAsset asset, int precision)>();
        }

        public  async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var result = new Dictionary<BlockchainAsset, decimal>();

            var page = 0;
            var proceedNext = true;

            var history = new List<AccountHistoryResponse>();
            while (proceedNext)
            {
                var batch = await _baseUrl.AppendPathSegment("account_history").SetQueryParams
                (
                    new
                    {
                        account_id = address,
                        page
                    }
                ).GetJsonAsync<AccountHistoryResponse[]>();
                
                history.AddRange(batch);
                page++;
                
                proceedNext = batch.Any();
            }

            decimal Align(decimal value, int precision)
            {
                return value / (decimal) (Math.Pow(10, precision));
            }

            foreach (var entry in history
                .Where(p => p.Timestamp <= at && p.Op.Amount != null)
                .OrderByDescending(p => p.Timestamp))
            {
                var assetInfo = await GetAssetInfoAsync(entry.Op.Amount.AssetId);

                var alignedAmount = Align(entry.Op.Amount.Value, assetInfo.precision);

                decimal balanceChange;

                var isIncomingAmount = string.Equals(address, entry.Op.To);
                if (isIncomingAmount)
                {
                    balanceChange = alignedAmount;
                }
                else
                {
                    var feeAssetInfo  = await GetAssetInfoAsync(entry.Op.Fee.AssetId);
                    var alignedFeeAmount =  Align(entry.Op.Fee.Value, feeAssetInfo.precision);

                    var feeSum = result.ContainsKey(assetInfo.asset) ? result[assetInfo.asset] : 0m;
                    feeSum -= alignedFeeAmount;
                    result[feeAssetInfo.asset] = feeSum;

                    balanceChange = alignedAmount * -1;
                }

                var sum = result.ContainsKey(assetInfo.asset) ? result[assetInfo.asset] : 0m;
                sum += balanceChange;
                result[assetInfo.asset] = sum;
            }

            return result;
        }

        private async Task<(BlockchainAsset asset, int precision)> GetAssetInfoAsync(string assetId)
        {
            if (_cachedAssets.ContainsKey(assetId))
            {
                return _cachedAssets[assetId];
            }

            (BlockchainAsset asset, int precision) result;
            if (_predefinedAssets.TryGetValue(assetId, out var data))
            {
                result =  (asset: new BlockchainAsset(data.assetName, assetId, data.lykkeAssetId), precision: 5);
            }
            else
            {
                var resp = await _baseUrl.AppendPathSegment("asset").SetQueryParams
                (
                    new
                    {
                        asset_id = assetId
                    }
                ).GetJsonAsync<AssetResponse>();

                result = (new BlockchainAsset(resp.Symbol, assetId, null), resp.Precision);
            }

            _cachedAssets[assetId] = result;
            return result;
        }
    }
}
