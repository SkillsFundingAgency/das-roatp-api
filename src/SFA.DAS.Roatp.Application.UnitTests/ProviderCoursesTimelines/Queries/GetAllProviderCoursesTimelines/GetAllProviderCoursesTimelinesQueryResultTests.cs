using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryResultTests
{
    [Test]
    public void Operator_ConvertsFromProviderRegistrationDetails()
    {
        var providerRegistrationDetails = new List<Domain.Entities.ProviderRegistrationDetail>
        {
            TestDataHelper.GetProviderRegistrationDetails()
        };
        GetAllProviderCoursesTimelinesQueryResult result = providerRegistrationDetails;
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Providers.Count());
    }
}
