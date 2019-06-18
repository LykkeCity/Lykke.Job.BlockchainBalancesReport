namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.LiteCoin
{
    public class LiteCoinExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "LiteCoin";
        public string Format(string address, string asset)
        {
            return $"https://blockchair.com/litecoin/address/{address}";
        }
    }
}
