using System;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Blockchains;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Bitcoin;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.BitcoinCash;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.BitcoinGold;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Dash;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Eos;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Kin;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.LiteCoin;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Ripple;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.Stellar;
using Lykke.Tools.BlockchainBalancesReport.Blockchains.ZCash;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
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
            services.AddTransient<IBalanceProvider, RippleBalanceProvider>();
            services.AddTransient<IBalanceProvider, BitcoinCashBalanceProvider>();
            services.AddTransient<IBalanceProvider, LiteCoinBalanceProvider>();
            services.AddTransient<IBalanceProvider, BitcoinGoldBalanceProvider>();
            services.AddTransient<IBalanceProvider, ZCashBalanceProvider>();
            services.AddTransient<IBalanceProvider, DashBalanceProvider>();
            services.AddTransient<IBalanceProvider, EosBalanceProvider>();
            services.AddTransient<IBalanceProvider, StellarBalanceProvider>();
            services.AddTransient<IBalanceProvider, KinBalanceProvider>();

            services.AddTransient<IExplorerUrlFormatter, BitcoinExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, RippleExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, BitcoinCashExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, LiteCoinExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, BitcoinGoldExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, ZCashExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, DashExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, EosExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, StellarExplorerUrlFormatter>();
            services.AddTransient<IExplorerUrlFormatter, KinExplorerUrlFormatter>();

            services.Configure<ReportSettings>(configuration.GetSection("Report"));
            services.Configure<BitcoinSettings>(configuration.GetSection("Bitcoin"));
            services.Configure<RippleSettings>(configuration.GetSection("Ripple"));
            services.Configure<BitcoinCashSettings>(configuration.GetSection("BitcoinCash"));
            services.Configure<LiteCoinSettings>(configuration.GetSection("LiteCoin"));
            services.Configure<BitcoinGoldSettings>(configuration.GetSection("BitcoinGold"));
            services.Configure<ZCashSettings>(configuration.GetSection("ZCash"));
            services.Configure<DashSettings>(configuration.GetSection("Dash"));
            services.Configure<EosSettings>(configuration.GetSection("Eos"));
            services.Configure<StellarSettings>(configuration.GetSection("Stellar"));
            services.Configure<KinSettings>(configuration.GetSection("Kin"));

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
