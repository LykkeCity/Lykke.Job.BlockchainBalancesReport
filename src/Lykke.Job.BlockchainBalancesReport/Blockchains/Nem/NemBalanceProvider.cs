using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Job.BlockchainBalancesReport.Clients.Nemchina;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Nem
{
    public class NemBalanceProvider : IBalanceProvider
    {
        private readonly string _baseUrl;
        private readonly BlockchainAsset _nemAsset;
        private const int Precision = 6;

        // ReSharper disable once UnusedMember.Global
        public NemBalanceProvider(NemSettings settings) :
            this(settings.NemChinaBaseUrl)
        {
        }

        public NemBalanceProvider(string baseUrl)
        {
            _baseUrl = baseUrl;
            
            _nemAsset = new BlockchainAsset("XEM", "XEM", "903eafbd-cc29-4d60-8d7d-907695d9caae");
        }

        public string BlockchainType => "Nem";

        public  async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var result = 0m;
            var page = 0;
            var proccedNext = true;

            var history = new List<TransactionsResponse>();
            while (proccedNext)
            {
                page++;
                var batch = await _baseUrl.AppendPathSegment("/account/detailTXList").PostJsonAsync
                (
                    new
                    {
                        address,
                        page
                    }
                ).ReceiveJson<TransactionsResponse[]>();
                
                history.AddRange(batch);
                
                proccedNext = batch.Any();
            }

            decimal Align(decimal value)
            {
                return value / (decimal) (Math.Pow(10, Precision));
            }

            var nemEpoch = new DateTimeOffset(2015, 03, 29, 0, 6, 25, TimeSpan.Zero).ToUnixTimeSeconds();

            foreach (var entry in history.Where(p => DateTimeOffset.FromUnixTimeSeconds(p.TimeStamp + nemEpoch) <= at))
            {

                var alignedAmount = Align(entry.Amount);

                decimal balanceChange;

                var isIncomingAmount = string.Equals(address, entry.Recipient);
                if (isIncomingAmount)
                {
                    balanceChange = alignedAmount;
                }
                else
                {
                    alignedAmount += Align(entry.Fee);
                    balanceChange = alignedAmount * -1;
                }
                result += balanceChange;
            }

            return new Dictionary<BlockchainAsset, decimal>
            {
                {_nemAsset, result}
            };
        }
    }
}
