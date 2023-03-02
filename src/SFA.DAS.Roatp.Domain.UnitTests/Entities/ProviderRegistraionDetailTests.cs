using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [TestFixture]
    public class ProviderRegistraionDetailTests
    {
        [Test]
        [AutoData]
        public void UpdateAddress_UpdatesAddressFromModel(UkrlpProviderAddress source, ProviderRegistrationDetail sut)
        {
            sut.UpdateAddress(source);

            Assert.That(sut.AddressLine1, Is.EqualTo(source.Address1));
            Assert.That(sut.AddressLine2, Is.EqualTo(source.Address2));
            Assert.That(sut.AddressLine3, Is.EqualTo(source.Address3));
            Assert.That(sut.AddressLine4, Is.EqualTo(source.Address4));
            Assert.That(sut.Town, Is.EqualTo(source.Town));
            Assert.That(sut.Postcode, Is.EqualTo(source.Postcode));
        }
    }
}
