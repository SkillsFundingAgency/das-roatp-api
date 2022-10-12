using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using System;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities;

[TestFixture]
public class ProviderAddressTests
{
    [Test, AutoData]
    public void ImplicitOperator_ConstructsObject(UkrlpProviderAddress source)
    {
        var destination = (ProviderAddress)source;

        destination.Id.Should().Be(source.Id);
        destination.ProviderId.Should().Be(source.ProviderId);
        destination.AddressLine1 = source.Address1;
        destination.AddressLine2 = source.Address2;
        destination.AddressLine3 = source.Address3;
        destination.AddressLine4 = source.Address4;
        destination.Town = source.Town;
        destination.Postcode = source.Postcode;
        destination.Latitude.Should().BeNull();
        destination.Longitude.Should().BeNull();
        destination.CoordinatesUpdateDate.Should().BeNull(); 
        Assert.That(destination.AddressUpdateDate, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(5.0)));
    }
}