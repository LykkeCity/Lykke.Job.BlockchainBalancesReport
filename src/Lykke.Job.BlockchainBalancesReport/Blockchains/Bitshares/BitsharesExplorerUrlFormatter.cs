namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Bitshares
{
    public class BitsharesExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Bitshares";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://bitshares-explorer.io/#/accounts/{address}";
        }
    }
}
