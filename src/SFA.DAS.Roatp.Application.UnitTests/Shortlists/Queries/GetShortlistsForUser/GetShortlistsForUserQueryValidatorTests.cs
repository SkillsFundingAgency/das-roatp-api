using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Queries.GetShortlistsForUser;

public class GetShortlistsForUserQueryValidatorTests
{
    [Test]
    public void Validate_WhenUserIdIsEmpty_ThenReturnsFalse()
    {
        var query = new GetShortlistsForUserQuery(Guid.Empty);
        var validator = new GetShortlistsForUserQueryValidator();
        var result = validator.Validate(query);
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WhenUserIdIsNotEmpty_ThenReturnsTrue()
    {
        var query = new GetShortlistsForUserQuery(Guid.NewGuid());
        var validator = new GetShortlistsForUserQueryValidator();
        var result = validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }
}
