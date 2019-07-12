using Autofac;
using Lykke.Job.BlockchainBalancesReport.Services;
using Lykke.Sdk;
using Lykke.Sdk.Health;

namespace Lykke.Job.BlockchainBalancesReport.Modules
{
    public class JobModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
