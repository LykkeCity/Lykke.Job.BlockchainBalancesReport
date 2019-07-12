﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Blockchains.Nem;
using Xunit;

namespace Lykke.Job.BlockchainBalancesReport.Tests
{
    public class NemProviderTests
    {
        [Fact]
        public async Task CanCalculateBalanceAtPointOfTime()
        {
            var balanceProvider = new NemBalanceProvider("http://explorer.nemchina.com/");

            var nemAsset = new Asset("XEM", "XEM", "903eafbd-cc29-4d60-8d7d-907695d9caae");
            
            var expectations = new List<(string address, DateTime dateTime, IReadOnlyDictionary<Asset, decimal> result)>
            {
                ("NAFSSJLNTIEI5ISMWKEY2BJFH5LAUSHP7JVQLWGT", DateTime.Parse("2019-04-07T19:00:00+0000"), new Dictionary<Asset, decimal>
                {
                    {nemAsset, 4868.073614m }
                })
            };

            foreach (var assert in expectations)
            {
                var result = await balanceProvider.GetBalancesAsync(assert.address, assert.dateTime);

                Assert.Equal(assert.result[nemAsset], result[nemAsset]);
            }
        }
    }
}