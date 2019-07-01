using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.NeoScan;
using Xunit;

namespace Tests
{
    public class NeoProviderTests
    {
        [Fact]
        public async Task CanCalculateBalanceAtPointOfTime()
        {
            var balanceProvider = new NeoScanClient("https://neoscan.io/api/main_net/v1/");

            var expectations = new List<(string address, DateTime dateTime, IReadOnlyDictionary<string, decimal> result)>
            {
                ("AYCFkFWhpxXgGzFjnMofYcJMUJ9Z8eneV3", DateTime.Parse("2019-07-01T19:00:00+0000"), new Dictionary<string, decimal>
                {
                    {"NEO", 1289 },
                    {"GAS", 4.30532041m },
                    {"SEAS", 0 }
                }),
                ("AYCFkFWhpxXgGzFjnMofYcJMUJ9Z8eneV3", DateTime.Parse("2019-07-01T12:23:50+0000"), new Dictionary<string, decimal>
                {
                    {"NEO", 1259 },
                    {"GAS", 0 },
                    {"SEAS", 0 }
                }),
            };

            foreach (var assert in expectations)
            {
                var result = await balanceProvider.GetBalanceAsync(assert.address, assert.dateTime);

                Assert.Equal(result, assert.result);
            }
        }
    }
}
