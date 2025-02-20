using System;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistCountForUser;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Queries.GetShortlistsCountForUser;

public class GetShortlistsCountForUserQueryValidatorTests
{
    [Test]
    public void Validate_UserIdEmpty_Invalid()
    {
        GetShortlistsCountForUserQueryValidator sut = new();

        var result = sut.TestValidate(new GetShortlistsCountForUserQuery(Guid.Empty));

        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Test]
    public void Validate_UserIdGiven_Valid()
    {
        GetShortlistsCountForUserQueryValidator sut = new();

        var result = sut.TestValidate(new GetShortlistsCountForUserQuery(Guid.NewGuid()));

        result.ShouldNotHaveValidationErrorFor(c => c.UserId);
    }
}
