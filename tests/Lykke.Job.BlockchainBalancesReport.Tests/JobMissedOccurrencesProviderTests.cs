using System;
using System.Linq;
using Cronos;
using Lykke.Job.BlockchainBalancesReport.Services;
using Lykke.Job.BlockchainBalancesReport.Utils;
using Xunit;

namespace Lykke.Job.BlockchainBalancesReport.Tests
{
    public class JobMissedOccurrencesProviderTests
    {
        private readonly JobMissedOccurrencesProvider _provider;

        public JobMissedOccurrencesProviderTests()
        {
            _provider = new JobMissedOccurrencesProvider();
        }

        [Theory]
        [InlineData("0 1 * * *", "2019-07-13T11:24:00", "2019-07-13T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-14T00:59:59.99999", "2019-07-13T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-13T01:00:00", "2019-07-13T01:00:00")]
        public void TestFirstRun(string cronExpression, string nowDateTime, string expectedDateTime)
        {
            // Arrange

            var cron = CronExpression.Parse(cronExpression);
            var lastOccurence = default(DateTime?);
            var now = DateTime.Parse(nowDateTime).AsUtc();

            // Act

            var missedOccurrences = _provider.GetMissedOccurrenceAsync(cron, lastOccurence, now);

            // Assert

            var expected = DateTime.Parse(expectedDateTime).AsUtc();

            Assert.NotNull(missedOccurrences);
            Assert.Equal(1, missedOccurrences.Count);
            Assert.Equal(expected, missedOccurrences.Single());
        }

        [Theory]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-12T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-13T00:59:59.99999")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-12T11:42:00")]
        public void TestPrematureRun(
            string cronExpression, 
            string lastOccurrenceDateTime, 
            string nowDateTime)
        {
            // Arrange

            var cron = CronExpression.Parse(cronExpression);
            var lastOccurence = DateTime.Parse(lastOccurrenceDateTime).AsUtc();
            var now = DateTime.Parse(nowDateTime).AsUtc();

            // Act

            var missedOccurrences = _provider.GetMissedOccurrenceAsync(cron, lastOccurence, now);

            // Assert

            Assert.NotNull(missedOccurrences);
            Assert.Equal(0, missedOccurrences.Count);
        }
        
        [Theory]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-13T11:24:00", "2019-07-13T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-14T00:59:59.99999", "2019-07-13T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-13T01:00:00", "2019-07-13T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-14T11:24:00", "2019-07-13T01:00:00,2019-07-14T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-15T00:59:59.99999", "2019-07-13T01:00:00,2019-07-14T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-14T01:00:00", "2019-07-13T01:00:00,2019-07-14T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-15T11:24:00", "2019-07-13T01:00:00,2019-07-14T01:00:00,2019-07-15T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-16T00:59:59.99999", "2019-07-13T01:00:00,2019-07-14T01:00:00,2019-07-15T01:00:00")]
        [InlineData("0 1 * * *", "2019-07-12T01:00:00", "2019-07-15T01:00:00", "2019-07-13T01:00:00,2019-07-14T01:00:00,2019-07-15T01:00:00")]
        public void TestMissedOccurrencesRun(
            string cronExpression, 
            string lastOccurrenceDateTime, 
            string nowDateTime,
            string expectedDateTimes)
        {
            // Arrange

            var cron = CronExpression.Parse(cronExpression);
            var lastOccurence = DateTime.Parse(lastOccurrenceDateTime).AsUtc();
            var now = DateTime.Parse(nowDateTime).AsUtc();

            // Act

            var missedOccurrences = _provider.GetMissedOccurrenceAsync(cron, lastOccurence, now);

            // Assert

            var expectedOccurrences = expectedDateTimes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => DateTime.Parse(x).AsUtc())
                .ToArray();

            Assert.NotNull(missedOccurrences);
            Assert.Equal(expectedOccurrences, missedOccurrences);
        }
    }
}
