using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Blockchains.Steem;
using Lykke.Job.BlockchainBalancesReport.Clients.Steemit;
using Xunit;

namespace Lykke.Job.BlockchainBalancesReport.Tests
{
    public class SteemProviderTests
    {
        [Fact]
        public async Task CanCalculateBalanceAtPointOfTime()
        {
            var balanceProvider = new SteemBalanceProvider("https://api.steemit.com/");

            var asset = new BlockchainAsset("STEEM", "STEEM", "72da9464-49d0-4f95-983d-635c04e39f3c");
            
            var expectations = new List<(string address, DateTime dateTime, IReadOnlyDictionary<BlockchainAsset, decimal> result)>
            {
                ("lykke-exchange", DateTime.Parse("2019-07-10T19:00:00+0000"), new Dictionary<BlockchainAsset, decimal>
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
            var respExample =await File.ReadAllTextAsync("SteemitResp.json");

            var typed = SteemitDeserializer.DeserializeTransactionsResp(respExample).ToList();

            Assert.NotEmpty(typed);

            foreach (var item in typed)
            {
                Assert.False(string.IsNullOrEmpty(item.txId));
                Assert.False(string.IsNullOrEmpty(item.from));
                Assert.False(string.IsNullOrEmpty(item.to));
            }
        }
    }
}
