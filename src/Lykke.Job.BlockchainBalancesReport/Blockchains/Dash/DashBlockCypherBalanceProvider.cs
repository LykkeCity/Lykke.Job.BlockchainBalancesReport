using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Clients.BlockCypherApi;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Polly;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Dash
{
    public class DashBlockCypherBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Dash";

        private readonly ILog _log;
        private readonly BlockCypherApiClient _client;
        
        public DashBlockCypherBalanceProvider(
            ILogFactory logFactory,
            DashSettings settings)
        {
            _log = logFactory.CreateLog(this);
            _client = new BlockCypherApiClient(settings.BlockCypherApiUrl);
        }

        public async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var before = 0L;
            var balance = 0L;

            do
            {
                var response = await Policy
                    .Handle<Exception>(ex =>
                    {
                        _log.Warning($"Failed to get address {address} data before block {before}. Operation will be retried.", ex);
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

            return new Dictionary<Asset, decimal>
            {
                {new Asset("DASH", "DASH", "4d498e43-956f-45ee-be07-8bb435003f26"), balance / 100000000M}
            };
        }
    }
}
