using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Ninja
{
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
