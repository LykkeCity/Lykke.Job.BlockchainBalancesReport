﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiTransactionsResponse
    {
        [JsonProperty("pagesTotal")]
        public int PagesTotal { get; set; }

        [JsonProperty("txs")]
        public IReadOnlyCollection<InsightApiTransaction> Transactions { get; set; }
    }
}
