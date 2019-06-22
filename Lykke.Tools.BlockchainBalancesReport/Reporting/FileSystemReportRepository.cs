using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;

namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public class FileSystemReportRepository : IReportRepository
    {
        private readonly ILogger<FileSystemReportRepository> _logger;
        private readonly string _filePath;

        public FileSystemReportRepository(
            ILogger<FileSystemReportRepository> logger,
            ReportFileRepositorySettings settings)
        {
            _logger = logger;
            _filePath = settings.FilePath;
        }

        public async Task SaveAsync(IReadOnlyCollection<ReportItem> items)
        {
            _logger.LogInformation($"Saving balances report to {_filePath}...");

            var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await writer.WriteLineAsync("date (UTC),blockchain,addressName,address,blockchain asset,asset ID,balance,explorer");

                foreach (var i in items.OrderBy(x => x.BlockchainType).ThenBy(x => x.AddressName))
                {
                    await writer.WriteAsync($"{i.Date:yyyy-MM-ddTHH:mm:ss},");
                    await writer.WriteAsync($"{i.BlockchainType},");
                    await writer.WriteAsync($"{i.AddressName},");
                    await writer.WriteAsync($"{i.Address},");
                    await writer.WriteAsync($"{i.BlockchainAsset},");
                    await writer.WriteAsync($"{i.AssetId},");
                    await writer.WriteAsync($"{i.Balance.ToString(CultureInfo.InvariantCulture)},");
                    await writer.WriteLineAsync(i.ExplorerUrl);
                }
            }

            _logger.LogInformation($"Balances report saving done. {items.Count} balances saved");
        }
    }
}
