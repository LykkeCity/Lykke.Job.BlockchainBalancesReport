namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Eos
{
    public class EosExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Eos";

        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://eosauthority.com/account/{address}";
        }
    }
}
