using System.Threading.Tasks;
using Common.Log;
using Cronos;
using Hangfire;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Jobs;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Lykke.Sdk;

namespace Lykke.Job.BlockchainBalancesReport.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly ScheduleSettings _scheduleSettings;

        public StartupManager(
            ILogFactory logFactory,
            ScheduleSettings scheduleSettings)
        {
            _scheduleSettings = scheduleSettings;
            _log = logFactory.CreateLog(this);
        }

        public Task StartAsync()
        {
            _log.Info($"Registering {nameof(BuildReportJob)} as recurring job '{BuildReportJob.Id}' with CRON '{_scheduleSettings.BuildReportCron}'...");

            RecurringJob.AddOrUpdate<BuildReportJob>
            (
                recurringJobId: BuildReportJob.Id,
                methodCall: job => job.ExecuteAsync(_scheduleSettings.BuildReportCron),
                cronExpression: _scheduleSettings.BuildReportCron
            );

            return Task.CompletedTask;
        }
    }
}
