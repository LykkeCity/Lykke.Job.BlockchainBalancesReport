using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Blockchains.Bitshares;
using Xunit;

namespace Lykke.Job.BlockchainBalancesReport.Tests
{
    public class BitsharesProviderTests
    {
        [Fact]
        public async Task CanCalculateBalanceAtPointOfTime()
        {
            var balanceProvider = new BitsharesBalanceProvider("https://explorer.bitshares-kibana.info/");

            var bitshresAsset = new Asset("BTS", "1.3.0", "20ce0468-917e-4097-abba-edf7c8600cfb");
            
            var expectations = new List<(string address, DateTime dateTime, IReadOnlyDictionary<Asset, decimal> result)>
            {
                ("1.2.1038643", DateTime.Parse("2019-07-03T19:00:00+0000"), new Dictionary<Asset, decimal>
                {
                    {bitshresAsset, 35441.40289m }
                })
            };

            foreach (var assert in expectations)
            {
                var result = await balanceProvider.GetBalancesAsync(assert.address, assert.dateTime);

                Assert.Equal(assert.result[bitshresAsset], result[bitshresAsset]);
            }
        }
    }
}
