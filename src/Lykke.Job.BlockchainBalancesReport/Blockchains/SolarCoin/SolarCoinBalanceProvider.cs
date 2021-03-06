﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Job.BlockchainBalancesReport.Clients.ChainId;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.SolarCoin
{
    public class SolarCoinBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "SolarCoin";


        private readonly string _baseUrl;
        private readonly BlockchainAsset _baseAsset;

        // ReSharper disable once UnusedMember.Global
        public SolarCoinBalanceProvider(SolarCoinSetting settings) :
            this(settings.ChainzBaseUrl)
        {
        }

        public SolarCoinBalanceProvider(string baseUrl)
        {
            _baseUrl = baseUrl;
            
            _baseAsset = new BlockchainAsset("SLR", "SLR", "SLR");
        }

        public  async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var indexResp = await (_baseUrl.AppendPathSegment("slr/address.dws") + $"?{address}.htm")
                                .GetStringAsync();

            var id = ChainIdDeserializer.GetChainid(indexResp);

            var txsResp = await _baseUrl.AppendPathSegment("explorer/address.summary.dws").SetQueryParams
                (
                    new
                    {
                        coin = "slr",
                        id
                    }
                )
                .GetStringAsync();

            if (txsResp.Contains("busy"))
            {
                throw new ArgumentException("Request failed due rate limiter");
            }

            var history = ChainIdDeserializer.DeserializeTransactionsResp(txsResp);
            var result = 0m;

            foreach (var entry in history.Where(p => p.date <= at))
            {
                result += entry.amount;
            }

            return new Dictionary<BlockchainAsset, decimal>
            {
                {_baseAsset, result}
            };
        }
    }
}
