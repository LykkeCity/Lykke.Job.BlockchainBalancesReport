using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Blockchains;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Steem;
using Newtonsoft.Json;
using Xunit;

namespace Tests
{
    public class SteemProviderTests
    {
        [Fact]
        public async Task CanCalculateBalanceAtPointOfTime()
        {
            var balanceProvider = new SteemBalanceProvider("https://api.steemit.com/");

            var asset = new Asset("STEEM", "STEEM", "72da9464-49d0-4f95-983d-635c04e39f3c");
            
            var expectations = new List<(string address, DateTime dateTime, IReadOnlyDictionary<Asset, decimal> result)>
            {
                ("lykke-exchange", DateTime.Parse("2019-04-07T19:00:00+0000"), new Dictionary<Asset, decimal>
                {
                    {asset, 2751.324m }
                })
            };

            foreach (var assert in expectations)
            {
                var result = await balanceProvider.GetBalancesAsync(assert.address, assert.dateTime);

                Assert.Equal(assert.result[asset], result[asset]);
            }
        }

        [Fact]
        public async Task TaskCanDeserializeSteemitResponse()
        {
            var respExample =await File.ReadAllTextAsync("SteemetResp.json");

            var typed = Deserialize(respExample).ToList();

            Assert.NotEmpty(typed);

            foreach (var item in typed)
            {
                Assert.False(string.IsNullOrEmpty(item.txId));
            }
        }

        private IEnumerable<(string txId, DateTime from)> Deserialize(string source)
        {
            var model = JsonConvert.DeserializeObject<SteemetResp>(source);
            var txs = model.Result.Select(p => p[1]).ToList();

            foreach (var tx  in txs)
            {
                var t = JsonConvert.SerializeObject(tx.op);
                var t2 = JsonConvert.DeserializeObject<dynamic[]>(t);
                if (t2[0] == "transfer")
                {

                    yield return (tx.trx_id, DateTime.Parse(tx.timestamp.ToString()));
                }
            }
        }

        public class SteemetResp
        {
            public dynamic[][] Result { get; set; }
        }
    }
}
