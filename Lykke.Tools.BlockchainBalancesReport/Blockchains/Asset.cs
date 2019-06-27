namespace Lykke.Tools.BlockchainBalancesReport.Blockchains
{
    public class Asset
    {
        public string BlockchainId { get; }
        public string LykkeId { get; }
        public string Name { get; }

        public Asset(string name, string blockchainId, string lykkeId)
        {
            Name = name;
            BlockchainId = blockchainId;
            LykkeId = lykkeId;
        }
    }
}
