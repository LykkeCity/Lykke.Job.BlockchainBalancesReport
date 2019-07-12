using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Lykke.Job.BlockchainBalancesReport.Clients.EosAuthorityApi
{
    public class EosAuthorityApiClient
    {
        private readonly string _url;

        public EosAuthorityApiClient(string url)
        {
            _url = url;
        }

        public async Task<EosAuthorityApiAccountGenesisResponse> GetAccountGenesisAsync(string account)
        {
            return await _url
                .AppendPathSegments("api", "spa", "account", account, "genesis")
                .SetQueryParams(new
                {
                    network = "eos"
                })
                .GetJsonAsync<EosAuthorityApiAccountGenesisResponse>();
        }
    }
}
