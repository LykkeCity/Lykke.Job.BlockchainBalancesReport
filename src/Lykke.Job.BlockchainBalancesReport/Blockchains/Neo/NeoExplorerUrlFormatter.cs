
namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Neo
{
    public class NeoExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Neo";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://neoscan.io/address/{address}";
        }
    }
}
