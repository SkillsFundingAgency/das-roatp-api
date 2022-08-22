using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderCourseAddModelTests
    {
        [Test, AutoData]
        public void Operator_TransformsToCommand(ProviderCourseAddModel source)
        {
            CreateProviderCourseCommand cmd = source;

            cmd.Should().BeEquivalentTo(source, options => options.ExcludingMissingMembers());
        }
    }
}
