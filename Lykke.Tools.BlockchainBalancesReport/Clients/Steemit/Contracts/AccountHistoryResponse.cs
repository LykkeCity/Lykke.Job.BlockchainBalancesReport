using System;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Steemit.Contracts
{
    public class AccountHistoryResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("result")]
        public ResultElement[][] Result { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public class ResultClass
    {
        [JsonProperty("trx_id")]
        public string TrxId { get; set; }

        [JsonProperty("block")]
        public long Block { get; set; }

        [JsonProperty("trx_in_block")]
        public long TrxInBlock { get; set; }

        [JsonProperty("op_in_trx")]
        public long OpInTrx { get; set; }

        [JsonProperty("virtual_op")]
        public long VirtualOp { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("op")]
        public OpElement[] Op { get; set; }
    }

    public class OpClass
    {
        [JsonProperty("fee", NullValueHandling = NullValueHandling.Ignore)]
        public string Fee { get; set; }


        [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
        public string From { get; set; }

        [JsonProperty("to", NullValueHandling = NullValueHandling.Ignore)]
        public string To { get; set; }

        [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
        public string Amount { get; set; }

        [JsonProperty("memo", NullValueHandling = NullValueHandling.Ignore)]
        public string Memo { get; set; }
    }


    public enum OpEnum { AccountCreate, Transfer };

    public struct OpElement
    {
        public OpEnum? Enum;
        public OpClass OpClass;

        public static implicit operator OpElement(OpEnum Enum) => new OpElement { Enum = Enum };
        public static implicit operator OpElement(OpClass OpClass) => new OpElement { OpClass = OpClass };
    }


    public struct ResultElement
    {
        public long? Integer;
        public ResultClass ResultClass;

        public static implicit operator ResultElement(long Integer) => new ResultElement { Integer = Integer };
        public static implicit operator ResultElement(ResultClass ResultClass) => new ResultElement { ResultClass = ResultClass };
    }
}
