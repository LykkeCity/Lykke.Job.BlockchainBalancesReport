using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.EosParkApi
{
    public class EosParkApiAccountTokensListResponseData
    {
        [JsonProperty("symbol_list")]
        public IReadOnlyCollection<EosParkApiAccountTokensListResponseToken> SymbolList { get; set; }
    }
}
