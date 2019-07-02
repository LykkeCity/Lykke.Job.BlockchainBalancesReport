using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Samurai
{
    public class SamuraiClient
    {
        private readonly string _url;

        public SamuraiClient(string url)
        {
            _url = url;
        }

        public async Task<SamuraiApiOperationsHistoryResponse> GetOperationsHistoryAsync(string address, int start, int count)
        {
            var response = await _url
                .AppendPathSegments("AddressHistory", address)
                .SetQueryParams(new
                {
                    Count = count, 
                    Start = start
                })
                .GetJsonAsync<SamuraiApiOperationsHistoryResponse>();

            return response;
        }

        public async Task<IReadOnlyCollection<SamuraiApiErc20Operation>> GetErc20OperationsHistory(string address, int start, int count)
        {
            var response = await _url
                .AppendPathSegments("Erc20TransferHistory", "getErc20Transfers", "v2")
                .SetQueryParams
                (
                    new
                    {
                        count = count,
                        start = start
                    }
                )
                .PostJsonAsync(new
                {
                    assetHolder = address
                });

            var responseContent = await response.Content.ReadAsStringAsync();
            var operations = JsonConvert.DeserializeObject<IReadOnlyCollection<SamuraiApiErc20Operation>>(responseContent);

            return operations;
        }

        public async Task<SamuraiErc20TokenResponse> GetErc20Token(string contractAddress)
        {
            var response = await _url
                .AppendPathSegments("Erc20Token", contractAddress)
                .GetJsonAsync<SamuraiErc20TokenResponse>();

            return response;
        }
    }
}
