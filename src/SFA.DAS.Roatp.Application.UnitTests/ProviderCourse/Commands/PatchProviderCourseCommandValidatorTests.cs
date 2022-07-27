using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands
{
    [TestFixture]
    public class PatchProviderCourseCommandValidatorTests
    {
        [TestCase(10000000, false)]
        [TestCase(10000001, true)]
        [TestCase(100000000, false)]
        public void Validate_Ukprn(int ukprn, bool isValid)
        {
            var validator = new PatchProviderCourseCommandValidator();

            var result = validator.TestValidate(new PatchProviderCourseCommand { Ukprn = ukprn, Patch = new JsonPatchDocument<PatchProviderCourse>()});

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
            var validator = new PatchProviderCourseCommandValidator();

            var result = validator.TestValidate(new PatchProviderCourseCommand { LarsCode = larsCode, Patch = new JsonPatchDocument<PatchProviderCourse>()});

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
            else
                result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [Test]
        public void Validate_Patch_NoOperations_ErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator();

            var result = validator.TestValidate(new PatchProviderCourseCommand { Patch = new JsonPatchDocument<PatchProviderCourse>() });

            result.ShouldHaveValidationErrorFor(c => c.Patch);
        }

        [Test]
        public void Validate_Patch_Operations_NoErrorMessage()
        {
            var validator = new PatchProviderCourseCommandValidator();

            var patch = new JsonPatchDocument<PatchProviderCourse>();
            patch.Add(r=>r.ContactUsEmail,"test@test.com");

            var result = validator.TestValidate(new PatchProviderCourseCommand { Patch = patch});

            result.ShouldNotHaveValidationErrorFor(c => c.Patch);
        }
    }
}
