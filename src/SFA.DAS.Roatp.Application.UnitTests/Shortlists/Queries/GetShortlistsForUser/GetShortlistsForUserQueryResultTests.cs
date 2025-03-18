using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Queries.GetShortlistsForUser;

public class GetShortlistsForUserQueryResultTests
{
    [Test, AutoData]
    public void ShortlistExpiryDate_Returns_MaxCreatedDate_Date_Plus_ShortlistExpiryDays(DateTime randomDate)
    {
        var queryResult = new GetShortlistsForUserQueryResult
        {
            MaxCreatedDate = randomDate
        };

        queryResult.ShortlistsExpiryDate.Date.Should().Be(randomDate.Date.AddDays(Application.Constants.ShortlistExpiryDays));
    }
}
