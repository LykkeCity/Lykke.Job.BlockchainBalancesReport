using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Lykke.Job.BlockchainBalancesReport.Clients.ChainId
{
    public static class ChainIdDeserializer
    {
        public static int GetChainid(string source)
        {
            var regex = new Regex(@"(?<=\baddrID=)[^,]*");
            var match = regex.Match(source);

            return int.Parse(match.Value);
        }

        public static IEnumerable<(string id, DateTime date, decimal amount)> DeserializeTransactionsResp(string source)
        {
            var json = JObject.Parse(source);
            var origin = 0;

            foreach (var tx in json["tx"].Select(p => p.AsJEnumerable().ToArray()))
            {
                var offset = tx[3].Value<int>();

                yield return (tx[1].Value<string>().ToLower(),
                    DateTimeOffset.FromUnixTimeSeconds(offset + origin).DateTime,
                    tx[4].Value<decimal>());

                origin += offset;
            }
        }
    }
}
