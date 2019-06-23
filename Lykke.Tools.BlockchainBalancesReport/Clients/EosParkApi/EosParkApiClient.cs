using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.EosParkApi
{
    public class EosParkApiClient
    {
        private readonly string _url;
        private readonly string _apiKey;

        public EosParkApiClient(string url, string apiKey)
        {
            _url = url;
            _apiKey = apiKey;
        }

        public async Task<EosParkApiAccountTransactionsResponse> GetAccountTransactions(string account, int page)
        {
            return await _url
                .SetQueryParams(new
                {
                    module = "account",
                    action = "get_account_related_trx_info",
                    apikey = _apiKey,
                    sort = 2,
                    account = account,
                    page = page
                })
                .GetJsonAsync<EosParkApiAccountTransactionsResponse>();
        }
    }
}
