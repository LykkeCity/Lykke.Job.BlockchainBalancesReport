namespace Lykke.Job.BlockchainBalancesReport.Blockchains
{
    public interface IExplorerUrlFormatter
    {
        string BlockchainType { get; }
        string Format(string address, BlockchainAsset asset);
    }
}
