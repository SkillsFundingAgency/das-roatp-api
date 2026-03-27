using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services;

public class ProviderEmailProcessingServiceTests
{
    [Test]
    public async Task SendEmailsInBatches_ProcessesBatches_WithDelayBetweenBatches()
    {
        Fixture fixture = new();
        var models = fixture
            .Build<ProviderEmailModel>()
            .With(m => m.Tokens, new Dictionary<string, string>() { { ProviderEmailTokens.Ukprn, "10012002" } })
            .CreateMany(ForecastEmailConfiguration.BatchSize + 1);

        Mock<ICourseManagementOuterApiClient> apiClientMock = new();

        var postCallTimes = new List<DateTime>();
        apiClientMock
            .Setup(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()))
            .Returns<string, ProviderEmailModel>((uri, model) =>
            {
                postCallTimes.Add(DateTime.UtcNow);
                return Task.FromResult(true);
            });

        ProviderEmailProcessingService sut = new(apiClientMock.Object);

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await sut.SendEmailsInBatches(models);
        sw.Stop();

        // Assert - ensure all posts happened
        apiClientMock.Verify(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()), Times.Exactly(models.Count()));
        postCallTimes.Should().HaveCount((models.Count()));
        // There should be a delay between the last call of the first batch (index of batch size -1) and the first call of the second batch (index of batch size )
        var gap = (postCallTimes[ForecastEmailConfiguration.BatchSize] - postCallTimes[ForecastEmailConfiguration.BatchSize - 1]).TotalMilliseconds;
        Assert.That(gap, Is.GreaterThanOrEqualTo(ForecastEmailConfiguration.EmailThrottlingInSeconds - 100), $"Expected gap >= 900ms between batches but was {gap}ms");
    }

    [Test]
    public void SendEmailsInBatches_ThrowsKeyNotFoundException_WhenUkprnTokenIsMissing()
    {
        // Arrange
        var model = new ProviderEmailModel("templateId", new Dictionary<string, string>()); // No Ukprn token
        Mock<ICourseManagementOuterApiClient> apiClientMock = new();
        ProviderEmailProcessingService sut = new(apiClientMock.Object);
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(() => sut.SendEmailsInBatches([model]));
    }
}
