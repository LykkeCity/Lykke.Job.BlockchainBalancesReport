namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Dash
{
    public class DashExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Dash";
        
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://blockchair.com/dash/address/{address}";
        }
    }
}
