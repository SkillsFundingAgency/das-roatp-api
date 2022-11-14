﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCourseCommandValidatorLarsCodeValidationTests : CreateProviderCourseCommandValidatorTestBase
    {
        [TestCase(0, false, LarsCodeUkprnAlreadyExistsValidator.InvalidMessage)]
        [TestCase(1, false, LarsCodeUkprnAlreadyExistsValidator.NotFoundMessage)]
        [TestCase(ValidComboLarsCode, false, LarsCodeUkprnAlreadyExistsValidator.CombinationAlreadyExistsMessage)]
        [TestCase(RegulatedLarsCode, true, "")]
        public async Task LarsCode_Validation(int larsCode, bool isValid, string expectedErrorMessage)
        {
            var command = new CreateProviderCourseCommand { LarsCode = larsCode, Ukprn = ValidUkprn };
            var sut = GetSut();

            var result = await sut.TestValidateAsync(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode).WithErrorMessage(expectedErrorMessage);
        }
    }
}
