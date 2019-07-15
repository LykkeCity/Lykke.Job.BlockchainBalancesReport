using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Blockchains.SolarCoin;
using Lykke.Job.BlockchainBalancesReport.Clients.ChainId;
using Xunit;

namespace Lykke.Job.BlockchainBalancesReport.Tests
{
    public class SolarCoinTests
    {
        [Fact]
        public async Task CanGetChainIdFromHtml()
        {
            var respExample = await File.ReadAllTextAsync("SolarCoinIndex.html");

            Assert.Equal(140227, ChainIdDeserializer.GetChainid(respExample));
        }

        [Fact]
        public async Task CanDeserializerTransactionsResp()
        {
            var respExample = await File.ReadAllTextAsync("SolarCoinTransactionsResp.json");

            var tx = ChainIdDeserializer.DeserializeTransactionsResp(respExample).Last();

            Assert.Equal("b63cf086b16e5d1570848e32bd65cc7747537cf4f3a32ac74b72d24c28adc54e", tx.id);

            Assert.Equal(DateTime.Parse("2019-07-14 16:58:16"), tx.date);
            Assert.Equal(-549.60515999m, tx.amount );
        }

        [Fact]
        public async Task CanCalculateBalanceAtPointOfTime()
        {
            var balanceProvider = new SolarCoinBalanceProvider("https://chainz.cryptoid.info");

            var baseAsset = new BlockchainAsset("SLR", "SLR", "SLR");

            var expectations = new List<(string address, DateTime dateTime, IReadOnlyDictionary<BlockchainAsset, decimal> result)>
            {
                ("8cSBrj3d9Hc2KZ6dfNCMHG4BqLwjMjNULP", DateTime.Parse("2019-07-14T22:00+0000"), new Dictionary<BlockchainAsset, decimal>
                {
                    {baseAsset, 7741781.94727671m }
                })
            };

            foreach (var assert in expectations)
            {
                var result = await balanceProvider.GetBalancesAsync(assert.address, assert.dateTime);

                Assert.Equal(assert.result[baseAsset], result[baseAsset]);
            }
        }
    }
}
