namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Eos
{
    public class EosExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Eos";

        public string Format(string address, Asset asset)
        {
            return $"https://eosauthority.com/account/{address}";
        }
    }
}
