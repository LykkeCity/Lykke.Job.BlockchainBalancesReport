using System.Threading.Tasks;

namespace Lykke.Job.BlockchainBalancesReport.Utils
{
    public interface IAsyncInitialization
    {
        Task AsyncInitialization { get; }
    }
}
