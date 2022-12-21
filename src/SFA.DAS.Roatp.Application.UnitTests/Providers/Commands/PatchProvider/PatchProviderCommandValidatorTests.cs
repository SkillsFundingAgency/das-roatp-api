using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using static SFA.DAS.Roatp.Application.Common.ValidationMessages;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.PatchProvider
{
    [TestFixture]
    public class PatchProviderCommandValidatorTests
    {
        private Mock<IProvidersReadRepository> _providersReadRepo;

        private const string MarketingInfo = "MarketingInfo";
        private const string Replace = "replace";

        [SetUp]
        public void Before_each_test()
        {
            _providersReadRepo = new Mock<IProvidersReadRepository>();
            _providersReadRepo.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());
        }

        [TestCase(10000000, false)]
        [TestCase(10000001, true)]
        [TestCase(100000000, false)]
        public async Task Validate_Ukprn(int ukprn, bool isValid)
        {
            var validator = new PatchProviderCommandValidator(_providersReadRepo.Object);

            var result = await validator.TestValidateAsync(new PatchProviderCommand { Ukprn = ukprn, Patch = new JsonPatchDocument<Domain.Models.PatchProvider>() });

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task Validate_PatchMarketingInfo_MatchingOperations_NoErrorMessage()
        {
            var validator = new PatchProviderCommandValidator(_providersReadRepo.Object);
            var ukprn = 10000001;

            var command = new PatchProviderCommand
            {
                Ukprn = ukprn,
                Patch = new JsonPatchDocument<Domain.Models.PatchProvider>()
            };

            command.Patch = new JsonPatchDocument<Domain.Models.PatchProvider>
            {
                Operations =
                {
                    new Operation<Domain.Models.PatchProvider>
                        { op = Replace, path = MarketingInfo, value = "Test-ProviderDescription" },
                }
            };

            var result = await validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public async Task Validate_Patch_Provider_MatchingOperationsWithUnavailableFieldOperation_UnavailableFieldErrorMessage()
        {
            var validator = new PatchProviderCommandValidator(_providersReadRepo.Object);
            var ukprn = 10000001;

            var command = new PatchProviderCommand
            {
                Ukprn = ukprn,
                Patch = new JsonPatchDocument<Domain.Models.PatchProvider>()
            };

            command.Patch = new JsonPatchDocument<Domain.Models.PatchProvider>
            {
                Operations =
                {
                    new Operation<Domain.Models.PatchProvider>
                        { op = Replace, path = MarketingInfo, value = "Test-ProviderDescription" },
                    new Operation<Domain.Models.PatchProvider>
                        { op = Replace, path = "unexpectedField", value = "field" }
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCommandValidator.PatchOperationContainsUnavailableFieldErrorMessage, result.Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Validate_Patch_Provider_MatchingOperationsWithUnavailableOperation_UnavailableFieldErrorMessage()
        {
            var validator = new PatchProviderCommandValidator(_providersReadRepo.Object);
            var ukprn = 10000001;

            var command = new PatchProviderCommand
            {
                Ukprn = ukprn,
                Patch = new JsonPatchDocument<Domain.Models.PatchProvider>()
            };

            command.Patch = new JsonPatchDocument<Domain.Models.PatchProvider>
            {
                Operations =
                {
                    new Operation<Domain.Models.PatchProvider>
                        { op = "Add", path = MarketingInfo, value = "Test-ProviderDescription" },
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCommandValidator.PatchOperationContainsUnavailableOperationErrorMessage, result.Errors[0].ErrorMessage);
        }


        [Test]
        public async Task Validate_Patch_NoOperations_ErrorMessage()
        {
            var validator = new PatchProviderCommandValidator(_providersReadRepo.Object);
            var ukprn = 10000001;

            var command = new PatchProviderCommand
            {
                Ukprn = ukprn,
                Patch = new JsonPatchDocument<Domain.Models.PatchProvider>()
            };

            var result = await validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Patch.Operations.Count);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.AreEqual(PatchProviderCommandValidator.NoPatchOperationsPresentErrorMessage, result.Errors[0].ErrorMessage);
        }


        [Test]
        public async Task Validate_Patch_Provider_DescriptionTooLong_ExpectedErrorMessage()
        {
            var validator = new PatchProviderCommandValidator(_providersReadRepo.Object);
            var ukprn = 10000001;

            var command = new PatchProviderCommand
            {
                Ukprn = ukprn,
                Patch = new JsonPatchDocument<Domain.Models.PatchProvider>()
            };

            command.Patch = new JsonPatchDocument<Domain.Models.PatchProvider>
            {
                Operations =
                {
                    new Operation<Domain.Models.PatchProvider>
                        { op = Replace, path = MarketingInfo, value = new string('x',750) + "ExtraText" },
                }
            };

            var result = await validator.TestValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(MarketingInfoValidationMessages.MarketingInfoTooLong));
        }
    }
}

