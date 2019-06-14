namespace Lykke.Tools.BlockchainBalancesReport.ExplorerUrlFormatters
{
    public class BitcoinExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Bitcoin";

        public string Format(string address, string asset)
        {
            return $"https://blockchair.com/bitcoin/address/{address}";
        }
    }
}
