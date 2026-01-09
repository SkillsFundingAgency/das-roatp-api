using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.PatchProvider;

[TestFixture]
public class PatchProviderCommandTests
{
    private const string Replace = "replace";
    private const string MarketingInfo = "MarketingInfo";

    [Test]
    public void Command_PatchContainsMarketingInfo_MarketingInfoIsSet()
    {
        var ukprn = 10000001;
        var testValue = "value";
        var patchCommand = new JsonPatchDocument<Domain.Models.PatchProvider>();
        patchCommand.Operations.Add(new Operation<Domain.Models.PatchProvider> { op = Replace, path = MarketingInfo, value = testValue });

        var command = new PatchProviderCommand
        {
            Ukprn = ukprn,
            Patch = patchCommand
        };

        testValue.Should().Be(command.MarketingInfo);
        command.IsPresentMarketingInfo.Should().BeTrue();
    }

    [Test]
    public void Command_PatchContainsNoDetails_FieldsAreNotSet()
    {
        var ukprn = 10000001;
        var patchCommand = new JsonPatchDocument<Domain.Models.PatchProvider>();

        var command = new PatchProviderCommand
        {
            Ukprn = ukprn,
            Patch = patchCommand
        };

        command.MarketingInfo.Should().BeNull();
        command.IsPresentMarketingInfo.Should().BeFalse();
    }
}