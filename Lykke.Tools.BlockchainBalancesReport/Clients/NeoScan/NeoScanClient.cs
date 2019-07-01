using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Tools.BlockchainBalancesReport.Clients.NeoScan.Contracts;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.NeoScan
{
    public class NeoScanClient
    {
        private readonly string _baseUrl;

        public NeoScanClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<IReadOnlyDictionary<string, decimal>> GetBalanceAsync(string address, DateTimeOffset at)
        {
            var result = new Dictionary<string, decimal>();

            BalanceResponse balance;
            try
            {
                balance = await GetJson<BalanceResponse>($"/get_balance/{address}");
            }
            catch (FlurlHttpException e) when(e.Call.HttpStatus == HttpStatusCode.NotFound)
            {
                return result;
            }
            
            foreach (var balanceByAsset in balance.Balances)
            {
                decimal sum = 0;

                foreach (var unspent in balanceByAsset.Unspents)
                {
                    var tx = await GetJson<TransactionResponse>($"get_transaction/{unspent.TxId}");

                    var date = DateTimeOffset.FromUnixTimeSeconds(tx.Timestamp);

                    if (date <= at)
                    {
                        sum += unspent.Value;
                    }
                }

                result.Add(balanceByAsset.AssetSymbol, sum);
            }

            return result;
        }

        private async Task<T> GetJson<T>(string segment)
        {
            return await _baseUrl.AppendPathSegment(segment).GetJsonAsync<T>();
        }
    }
}
