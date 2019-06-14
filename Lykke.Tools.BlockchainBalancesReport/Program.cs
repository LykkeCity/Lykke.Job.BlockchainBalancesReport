using System;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Balances;
using Lykke.Tools.BlockchainBalancesReport.Balances.Bitcoin;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Lykke.Tools.BlockchainBalancesReport.ExplorerUrlFormatters;
using Lykke.Tools.BlockchainBalancesReport.Reporting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lykke.Tools.BlockchainBalancesReport
{
    public class Program : IDisposable
    {
        private IServiceProvider ServiceProvider { get; }

        private Program()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddLogging(logging =>
            {
                logging.AddConsole();
            });

            services.AddSingleton<BalanceProvidersFactory>();
            services.AddSingleton<ExplorerUrlFormattersFactory>();

            services.AddSingleton<BalancesReport>();
            services.AddTransient<BalancesReportBuilder>();

            services.AddTransient<IBalanceProvider, BitcoinBalanceProvider>();

            services.AddTransient<IExplorerUrlFormatter, BitcoinExplorerUrlFormatter>();

            services.Configure<ReportSettings>(configuration.GetSection("Report"));
            services.Configure<BitcoinSettings>(configuration.GetSection("Bitcoin"));

            ServiceProvider = services.BuildServiceProvider();
        }

        // ReSharper disable once UnusedParameter.Local
        private static async Task Main(string[] args)
        {
            try
            {
                using (var program = new Program())
                {
                    await program.RunAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task RunAsync()
        {
            try
            {
                var reportBuilder = ServiceProvider.GetRequiredService<BalancesReportBuilder>();

                await reportBuilder.BuildAsync();
            }
            catch (Exception ex)
            {
                ServiceProvider.GetRequiredService<ILogger<Program>>().LogError(ex, string.Empty);
            }
        }

        public void Dispose()
        {
            (ServiceProvider as IDisposable)?.Dispose();
        }
    }
}
