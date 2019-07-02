
namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Neo
{
    public class NeoExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Neo";
        public string Format(string address, Asset asset)
        {
            return $"https://neoscan.io/address/{address}";
        }
    }
}
