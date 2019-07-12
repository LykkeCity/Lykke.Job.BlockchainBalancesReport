using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Lykke.Job.BlockchainBalancesReport.Clients.BlockCypherApi
{
    public class BlockCypherApiClient
    {
        private readonly string _url;

        public BlockCypherApiClient(string url)
        {
            _url = url;
        }

        public Task<BlockCypherApiAddressResponse> GetAddress(string address, long before, int limit)
        {
            return _url
                .AppendPathSegments("addrs", address)
                .SetQueryParams(new { limit, before })
                .GetJsonAsync<BlockCypherApiAddressResponse>();
        }
    }
}
