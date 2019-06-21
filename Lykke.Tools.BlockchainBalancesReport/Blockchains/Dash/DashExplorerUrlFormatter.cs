namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Dash
{
    public class DashExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Dash";
        
        public string Format(string address, string asset)
        {
            return $"https://blockchair.com/dash/address/{address}";
        }
    }
}