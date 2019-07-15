using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Lykke.Job.BlockchainBalancesReport.Clients.Steemit;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Steem
{
    public class SteemBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Steem";

        private readonly string _baseUrl;
        private readonly BlockchainAsset _steemAsset;

        // ReSharper disable once UnusedMember.Global
        public SteemBalanceProvider(SteemSettings settings) :
            this(settings.SteemitBaseUrl)
        {
        }

        public SteemBalanceProvider(string baseUrl)
        {
            _baseUrl = baseUrl;
            
            _steemAsset = new BlockchainAsset("STEEM", "STEEM", "72da9464-49d0-4f95-983d-635c04e39f3c");
        }
        
        public  async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
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

            var history = SteemitDeserializer.DeserializeTransactionsResp(resp).ToList();
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

            return new Dictionary<BlockchainAsset, decimal>
            {
                {_steemAsset, result}
            };
        }
    }
}
