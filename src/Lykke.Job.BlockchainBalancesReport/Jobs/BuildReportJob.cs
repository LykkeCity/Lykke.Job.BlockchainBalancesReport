using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Cronos;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Reporting;
using Lykke.Job.BlockchainBalancesReport.Services;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Jobs
{
    public class BuildReportJob
    {
        public const string Id = "06ddbc641d9f4c26b20ae23a3956d580";

        private static readonly SemaphoreSlim Lock;

        private readonly ILog _log;
        private readonly ReportSettings _reportSettings;
        private readonly LastReportOccurrenceRepository _lastReportOccurrenceRepository;
        private readonly BalancesReportBuilder _reportBuilder;

        static BuildReportJob()
        {
            Lock = new SemaphoreSlim(1);
        }

        public BuildReportJob(
            ILogFactory logFactory,
            ReportSettings reportSettings,
            LastReportOccurrenceRepository lastReportOccurrenceRepository,
            BalancesReportBuilder reportBuilder)
        {
            _reportSettings = reportSettings;
            _lastReportOccurrenceRepository = lastReportOccurrenceRepository;
            _reportBuilder = reportBuilder;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteAsync(string scheduleCronExpression)
        {
            if (!await Lock.WaitAsync(TimeSpan.FromMilliseconds(100)))
            {
                _log.Info("Job execution has been skipped because previous job is still executing");
                return;
            }

            try
            {
                var scheduleCron = CronExpression.Parse(scheduleCronExpression);

                _log.Info
                (
                    $"Executing the job. Schedule CRON: {scheduleCron}, balances interval: {_reportSettings.BalancesIntervalFromSchedule}"
                );

                var lastOccurrence = await _lastReportOccurrenceRepository.GetLastOccurrenceOrDefaultAsync();
                var now = DateTime.UtcNow;

                _log.Info($"Last report occurence is [{lastOccurrence:s}]");

                var missedOccurrencesProvider = new JobMissedOccurrencesProvider();
                var missedOccurrences = missedOccurrencesProvider.GetMissedOccurrenceAsync(scheduleCron, lastOccurrence, now);

                _log.Info("Missed occurrences", missedOccurrences);

                foreach (var occurrence in missedOccurrences)
                {
                    await _reportBuilder.BuildAsync(occurrence + _reportSettings.BalancesIntervalFromSchedule);
                    await _lastReportOccurrenceRepository.SaveLastOccurrenceAsync(occurrence);
                }
            }
            finally
            {
                Lock.Release();

                _log.Info("Job execution has been finished");
            }
        }
    }
}
