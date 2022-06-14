﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateApprovedByRegulator;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.UpdateApprovedByRegulator
{
    [TestFixture]
    public class UpdateApprovedByRegulatorCommandValidatorTests
    {
        [TestCase(10000000, false)]
        [TestCase(10000001, true)]
        [TestCase(100000000, false)]
        public void Validate_Ukprn(int ukprn, bool isValid)
        {
            var validator = new UpdateApprovedByRegulatorCommandValidator();

            var result = validator.TestValidate(new UpdateApprovedByRegulatorCommand { Ukprn = ukprn });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(-1, false)]
        public void Validate_LarsCode(int larsCode, bool isValid)
        {
            var validator = new UpdateApprovedByRegulatorCommandValidator();

            var result = validator.TestValidate(new UpdateApprovedByRegulatorCommand { LarsCode = larsCode });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        public void Validate_IsApprovedByRegulator(bool isApprovedByRegulator, bool isValid)
        {
            var validator = new UpdateApprovedByRegulatorCommandValidator();

            var result = validator.TestValidate(new UpdateApprovedByRegulatorCommand { Ukprn= 10000001, LarsCode = 1, IsApprovedByRegulator = isApprovedByRegulator });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.IsApprovedByRegulator);
            else
                result.ShouldHaveValidationErrorFor(c => c.IsApprovedByRegulator);
        }
    }
}
