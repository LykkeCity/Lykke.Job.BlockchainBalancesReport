﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.BlockCypherApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Dash
{
    public class DashBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Dash";

        private readonly ILogger<DashBalanceProvider> _logger;
        private readonly BlockCypherApiClient _client;

        public DashBalanceProvider(
            ILogger<DashBalanceProvider> logger,
            IOptions<DashSettings> settings)
        {
            _logger = logger;
            _client = new BlockCypherApiClient(settings.Value.BlockCypherApiUrl);
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var before = 0L;
            var balance = 0L;

            do
            {
                var response = await Policy
                    .Handle<Exception>(ex =>
                    {
                        _logger.LogWarning(ex, $"Failed to get address {address} data before block {before}. Operation will be retried.");
                        return true;
                    })
                    .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(Math.Min(i, 5)))
                    .ExecuteAsync(async () => await _client.GetAddress(address, before, limit: 0));

                foreach (var txRef in response.TxRefs)
                {
                    if (txRef.Confirmed > at)
                    {
                        continue;
                    }

                    balance += txRef.TxInputN >= 0 ? -txRef.Value : txRef.Value;
                    before = before != 0 ? Math.Min(before, txRef.BlockHeight) : txRef.BlockHeight;
                }

                if (!response.HasMore)
                {
                    break;
                }
            }
            while (true);

            return new Dictionary<(string BlockchainAsset, string AssetId), decimal>
            {
                {("DASH", "4d498e43-956f-45ee-be07-8bb435003f26"), balance / 100000000M}
            };
        }
    }
}
