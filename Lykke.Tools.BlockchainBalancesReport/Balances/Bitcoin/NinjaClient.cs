using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Lykke.Tools.BlockchainBalancesReport.Balances.Bitcoin
{
    public class NinjaBalancesResponse
    {
        public string Continuation { get; set; }
        public IReadOnlyCollection<NinjaBalancesOperation> Operations { get; set; }
    }

    public class NinjaBalancesOperation
    {
        public long Amount { get; set; }
        public DateTimeOffset FirstSeen { get; set; }
    }

    public class NinjaClient
    {
        private readonly string _url;

        public NinjaClient(string url)
        {
            _url = url;
        }

        public async Task<NinjaBalancesResponse> GetBalancesAsync(string address, bool unspentOnly, string continuation)
        {
            return await _url
                .AppendPathSegments("balances", address)
                .SetQueryParams(new
                {
                    unspentonly = unspentOnly,
                    continuation = continuation,
                    colored = false
                })
                .GetJsonAsync<NinjaBalancesResponse>();
        }
    }
}
