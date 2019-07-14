using System;
using Autofac;
using Hangfire;
using Hangfire.Mongo;
using Lykke.Job.BlockchainBalancesReport.Jobs;
using Lykke.Job.BlockchainBalancesReport.Services;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Lykke.Logs.Hangfire;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Service.Assets.Client;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainBalancesReport.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;

        public JobModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .WithParameter(TypedParameter.From(_settings.Schedule))
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<BuildReportJob>()
                .WithParameter(TypedParameter.From(_settings.Report))
                .AsSelf();

            if (_settings.Schedule.IsEnabled)
            {
                builder
                    .RegisterBuildCallback(StartHangfireServer)
                    .Register(ctx => new BackgroundJobServer())
                    .SingleInstance();
            }

            builder.RegisterAssetsClient(new AssetServiceSettings
            {
                AssetPairsCacheExpirationPeriod = _settings.Assets.CacheExpirationPeriod,
                AssetsCacheExpirationPeriod = _settings.Assets.CacheExpirationPeriod,
                Erc20TokensCacheExpirationPeriod = _settings.Assets.CacheExpirationPeriod,
                BaseUri = new Uri(_settings.Assets.ServiceUrl)
            });
        }

        private void StartHangfireServer(
            IContainer container)
        {
            GlobalConfiguration.Configuration
                .UseMongoStorage
                (
                    connectionString: _settings.MongoStorage.HangFireConnString,
                    databaseName: "BlockchainBalancesReportJob",
                    storageOptions: new MongoStorageOptions
                    {
                        CheckConnection = false,
                        MigrationOptions = new MongoMigrationOptions(MongoMigrationStrategy.Migrate)
                    }
                );
            
            GlobalConfiguration.Configuration
                .UseLykkeLogProvider(container)
                .UseAutofacActivator(container);

            container.Resolve<BackgroundJobServer>();
        }
    }
}
