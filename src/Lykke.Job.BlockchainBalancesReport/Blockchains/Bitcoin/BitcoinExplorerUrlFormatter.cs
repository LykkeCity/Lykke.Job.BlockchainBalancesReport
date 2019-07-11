namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Bitcoin
{
    public class BitcoinExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Bitcoin";

        public string Format(string address, Asset asset)
        {
            return $"https://blockchair.com/bitcoin/address/{address}";
        }
    }
}
