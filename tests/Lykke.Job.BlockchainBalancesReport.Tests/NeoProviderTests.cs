using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Neo;
using Lykke.Tools.BlockchainBalancesReport.Blockchains;
using Xunit;

namespace Tests
{
    public class NeoProviderTests
    {
        [Fact]
        public async Task CanCalculateBalanceAtPointOfTime()
        {
            var balanceProvider = new NeoBalanceProvider("https://neoscan.io/api/main_net/v1/");

            var neoAsset = new Asset("NEO", "c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b", "ac2e579f-187b-4429-8d60-bea6e4f65f76");
            var gasAsset = new Asset("GAS", "602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7", "f1ccf1dd-9008-4999-adc8-2cb587717083");
            var seasAsset = new Asset("SEAS", "de7be47c4c93f1483a0a3fff556a885a68413d97", null);

            var expectations = new List<(string address, DateTime dateTime, IReadOnlyDictionary<Asset, decimal> result)>
            {
                ("AYCFkFWhpxXgGzFjnMofYcJMUJ9Z8eneV3", DateTime.Parse("2019-07-01T19:00:00+0000"), new Dictionary<Asset, decimal>
                {
                    {neoAsset, 1289 },
                    {gasAsset, 4.30532041m },
                    {seasAsset, 12 }
                }),
                ("AYCFkFWhpxXgGzFjnMofYcJMUJ9Z8eneV3", DateTime.Parse("2019-07-01T12:23:50+0000"), new Dictionary<Asset, decimal>
                {
                    {neoAsset, 1259 },
                    {gasAsset, 0 },
                    {seasAsset, 12 }
                }),
            };

            foreach (var assert in expectations)
            {
                var result = await balanceProvider.GetBalancesAsync(assert.address, assert.dateTime);

                Assert.Equal(result, assert.result);
            }
        }
    }
}
