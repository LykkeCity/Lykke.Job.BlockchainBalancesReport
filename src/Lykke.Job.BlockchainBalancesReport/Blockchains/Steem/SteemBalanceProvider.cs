using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Lykke.Job.BlockchainBalancesReport.Clients.Steemit;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Microsoft.Extensions.Options;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Steem
{
    public class SteemBalanceProvider : IBalanceProvider
    {
        private readonly string _baseUrl;
        private readonly Asset _steemAsset;

        // ReSharper disable once UnusedMember.Global
        public SteemBalanceProvider(IOptions<SteemSettings> settings) :
            this(settings.Value.SteemetBaseUrl)
        {
        }

        public SteemBalanceProvider(string baseUrl)
        {
            _baseUrl = baseUrl;
            
            _steemAsset = new Asset("STEEM", "STEEM", "72da9464-49d0-4f95-983d-635c04e39f3c");
        }

        public string BlockchainType => "Steem";

        public  async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var result = 0m;

            var resp = await _baseUrl.PostJsonAsync
            (
                new
                {
                    id = Guid.NewGuid().ToString(),
                    jsonrpc = "2.0",
                    method = "call",
                    @params = new dynamic[] { "database_api", "get_account_history", new dynamic[] { address, -1, 2000 } }
                }
            ).ReceiveString();

            var history = SteemetDeserializer.DeserializeTransactionsResp(resp).ToList();
            foreach (var entry in history.Where(p => p.timestamp <= at))
            {
                var isIncomingAmount = string.Equals(address, entry.to, StringComparison.InvariantCultureIgnoreCase);
                if (isIncomingAmount)
                {
                    result += entry.amount;
                }
                else
                {
                    result -= entry.amount;
                }
            }

            return new Dictionary<Asset, decimal>
            {
                {_steemAsset, result}
            };
        }
    }
}
