
namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Decred
{
    public class BitsharesExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Bitshares";
        public string Format(string address, Asset asset)
        {
            return $"https://bitshares-explorer.io/#/accounts/{address}";
        }
    }
}
