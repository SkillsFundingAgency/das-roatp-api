using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse
{
    [TestFixture]
    public class CreateProviderCourseCommandTests
    {
        [Test, AutoData]
        public void Operator_TransformsCommandToEntity(CreateProviderCourseCommand sut)
        {
            Domain.Entities.ProviderCourse providerCourse = sut;

            providerCourse.Should().BeEquivalentTo(sut, option => option.ExcludingMissingMembers());
        }
    }
}
