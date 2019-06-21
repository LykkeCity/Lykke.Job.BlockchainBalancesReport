
namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.ZCash
{
    public class ZCashExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "ZCash";
        public string Format(string address, string asset)
        {
            return $"https://zcashnetwork.info/address/{address}";
        }
    }
}
