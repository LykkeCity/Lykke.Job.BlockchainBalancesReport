using System;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains
{
    public class Asset:IEquatable<Asset>
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

        public bool Equals(Asset other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
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

        public override string ToString()
        {
            return $"Asset : {Name}";
        }
    }
}
