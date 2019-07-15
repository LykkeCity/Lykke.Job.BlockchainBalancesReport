
namespace Lykke.Job.BlockchainBalancesReport.Blockchains.ZCash
{
    public class ZCashExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "ZCash";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://zcashnetwork.info/address/{address}";
        }
    }
}
