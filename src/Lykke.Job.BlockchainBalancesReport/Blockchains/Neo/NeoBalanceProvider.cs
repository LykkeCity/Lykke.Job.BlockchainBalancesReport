﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Job.BlockchainBalancesReport.Clients.NeoScan.Contracts;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Neo
{
    public class NeoBalanceProvider: IBalanceProvider
    {
        private const string NeoBlockchainAssetId = "c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b";
        private const string GasBlockchainAssetId = "602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7";

        public string BlockchainType => "Neo";

        private readonly string _baseUrl;

        private readonly BlockchainAsset _neoAsset;
        private readonly BlockchainAsset _gasAsset;
        private readonly Dictionary<string, string> _assetNames;

        // ReSharper disable once UnusedMember.Global
        public NeoBalanceProvider(NeoSettings settings) :
            this(settings.NeoScanBaseUrl)
        {
        }

        public NeoBalanceProvider(string baseUrl)
        {
            _baseUrl = baseUrl;

            _neoAsset = BuildAsset(NeoBlockchainAssetId);
            _gasAsset = BuildAsset(GasBlockchainAssetId);

            _assetNames = new Dictionary<string, string>
            {
                {"de7be47c4c93f1483a0a3fff556a885a68413d97", "SEAS"}
            };
        }

        public  async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var result = new Dictionary<BlockchainAsset, decimal>
            {
                {_neoAsset, 0},
                {_gasAsset, 0}
            };

            var page = 0;
            var proccedNext = true;
            while (proccedNext)
            {
                page++;
                var batch = await GetJson<GetAddressAbstractResponse>($"/get_address_abstracts/{address}/{page}");

                foreach (var entry in batch.Entries.Where(p => DateTimeOffset.FromUnixTimeSeconds(p.Time) <= at))
                {
                    var asset = BuildAsset(entry.Asset);

                    var sum = result.ContainsKey(asset) ? result[asset] : 0m;

                    var isIncomingAmount = string.Equals(address, entry.AddressTo);

                    if (isIncomingAmount)
                    {
                        sum += entry.Amount;
                    }
                    else
                    {
                        sum -= entry.Amount;
                    }

                    result[asset] = sum;
                }

                
                proccedNext = batch.Entries.Any();
            }

            return result;
        }


        private async Task<T> GetJson<T>(string segment)
        {
            return await _baseUrl.AppendPathSegment(segment).GetJsonAsync<T>();
        }

        private BlockchainAsset BuildAsset(string blockchainAssetId)
        {
            switch (blockchainAssetId)
            {
                case NeoBlockchainAssetId:
                {
                    return new BlockchainAsset("NEO", blockchainAssetId, "ac2e579f-187b-4429-8d60-bea6e4f65f76");
                }
                case GasBlockchainAssetId:
                {
                    return new BlockchainAsset("GAS", blockchainAssetId, "f1ccf1dd-9008-4999-adc8-2cb587717083");
                }
                default:
                {
                    if (!_assetNames.TryGetValue(blockchainAssetId, out var assetName))
                    {
                        assetName = blockchainAssetId;
                    }

                    return new BlockchainAsset(assetName, blockchainAssetId, null);
                }
            }
        } 
    }
}
