﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class ProviderLocationModelTest
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(ProviderLocation location)
        {
            var model = (ProviderLocationModel)location;

            model.Should().BeEquivalentTo(location, c => c
                 .Excluding(s => s.Id)
                 .Excluding(s => s.ImportedLocationId)
                 .Excluding(s => s.ProviderId)
                 .Excluding(s => s.RegionId)
                 .Excluding(s => s.Provider)
                 .Excluding( s=>s.ProviderCourseLocations)
                 .Excluding(s => s.Region)
            );
        }
    }
}