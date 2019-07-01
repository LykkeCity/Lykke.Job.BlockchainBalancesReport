namespace Lykke.Tools.BlockchainBalancesReport.Blockchains
{
    public class Asset
    {
        protected bool Equals(Asset other)
        {
            return string.Equals(BlockchainId, other.BlockchainId) && string.Equals(LykkeId, other.LykkeId) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Asset) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (BlockchainId != null ? BlockchainId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LykkeId != null ? LykkeId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

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
