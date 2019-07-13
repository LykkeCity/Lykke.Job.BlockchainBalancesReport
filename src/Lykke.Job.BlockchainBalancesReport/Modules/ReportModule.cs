using System.Reflection;
using Autofac;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Blockchains.Dash;
using Lykke.Job.BlockchainBalancesReport.Reporting;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Lykke.SettingsReader;
using Module = Autofac.Module;

namespace Lykke.Job.BlockchainBalancesReport.Modules
{
    public class ReportModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IReloadingManager<AzureStorageSettings> _azureStorageSettings;

        public ReportModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue;
            _azureStorageSettings = settings.Nested(s => s.AzureStorage);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BalanceProvidersFactory>().AsSelf().SingleInstance();
            builder.RegisterType<ExplorerUrlFormattersFactory>().AsSelf().SingleInstance();

            builder.RegisterType<BalancesReport>()
                .AsSelf()
                .WithParameter(TypedParameter.From(_settings.Report));
            builder.RegisterType<BalancesReportBuilder>()
                .WithParameter(TypedParameter.From(_settings.Report))
                .AsSelf();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AssignableTo<IBalanceProvider>()
                .Except<DashBlockCypherBalanceProvider>()
                .As<IBalanceProvider>();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AssignableTo<IExplorerUrlFormatter>()
                .As<IExplorerUrlFormatter>();

            RegisterBlockchainSettings(builder);

            builder.RegisterType<LastReportOccurrenceRepository>()
                .AsSelf()
                .WithParameter(TypedParameter.From(_azureStorageSettings.ConnectionString(s => s.DataConnString)))
                .SingleInstance();
        }

        private void RegisterBlockchainSettings(ContainerBuilder builder)
        {
            const BindingFlags propertiesBindingFlags = BindingFlags.Instance |
                                                        BindingFlags.Public |
                                                        BindingFlags.GetProperty |
                                                        BindingFlags.SetProperty;
            var settingsType = _settings.Blockchains.GetType();
            var properties = settingsType.GetProperties(propertiesBindingFlags);

            foreach (var property in properties)
            {
                builder.RegisterInstance(property.GetValue(_settings.Blockchains))
                    .As(property.PropertyType);
            }
        }
    }
}
