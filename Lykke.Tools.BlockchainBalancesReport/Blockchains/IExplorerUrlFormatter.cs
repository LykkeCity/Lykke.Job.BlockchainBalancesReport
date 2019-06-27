namespace Lykke.Tools.BlockchainBalancesReport.Blockchains
{
    public interface IExplorerUrlFormatter
    {
        string BlockchainType { get; }
        string Format(string address, Asset asset);
    }
}
