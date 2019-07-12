using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class FileSystemReportRepository : IReportRepository
    {
        private readonly ILog _logger;
        private readonly string _filePath;

        public FileSystemReportRepository(
            ILogFactory logFactory,
            ReportFileRepositorySettings settings)
        {
            _logger = logFactory.CreateLog(this);
            _filePath = settings.FilePath;
        }

        public async Task SaveAsync(IReadOnlyCollection<ReportItem> items)
        {
            _logger.Info($"Saving balances report to {_filePath}...");

            var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await writer.WriteLineAsync("date (UTC),blockchain,addressName,address,asset,balance,blockchain asset ID,asset ID,explorer");

                foreach (var i in items.OrderBy(x => x.BlockchainType).ThenBy(x => x.AddressName))
                {
                    await writer.WriteAsync($"{i.Date:yyyy-MM-ddTHH:mm:ss},");
                    await writer.WriteAsync($"{i.BlockchainType},");
                    await writer.WriteAsync($"{i.AddressName},");
                    await writer.WriteAsync($"{i.Address},");
                    await writer.WriteAsync($"{i.Asset.Name},");
                    await writer.WriteAsync($"{i.Balance.ToString(CultureInfo.InvariantCulture)},");
                    await writer.WriteAsync($"{i.Asset.BlockchainId},");
                    await writer.WriteAsync($"{i.Asset.LykkeId},");
                    await writer.WriteLineAsync(i.ExplorerUrl);
                }
            }

            _logger.Info($"Balances report saving done. {items.Count} balances saved");
        }
    }
}
