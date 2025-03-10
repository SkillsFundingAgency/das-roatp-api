using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories;

public sealed class ProviderRegistrationDetailsReadRepositoryTests
{
    private RoatpDataContext? _inMemoryContext;

    [SetUp]
    public void Setup()
    {
        _inMemoryContext = RoatpDataContextFactory.CreateInMemoryContext();

        SeedData();
    }

    private void SeedData()
    {
        var providerRegistrationDetails = new List<ProviderRegistrationDetail>
        {
            new ProviderRegistrationDetail
            {
                StatusId = OrganisationStatus.Active,
                ProviderTypeId = ProviderType.Main,
                Provider = new Provider
                {
                    Ukprn = 1,
                    LegalName = "Valid provider",
                    Locations = new List<ProviderLocation>
                    {
                        new ProviderLocation
                        {
                            Latitude = 0,
                            Longitude = 0,
                            ProviderCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation() }
                        }
                    },
                    Courses = new List<ProviderCourse> { new ProviderCourse() }
                }
            },
            new ProviderRegistrationDetail
            {
                StatusId = OrganisationStatus.Active,
                ProviderTypeId = ProviderType.Main,
                Provider = new Provider
                {
                    Ukprn = 2,
                    LegalName = "Provider with no locations",
                    Locations = new List<ProviderLocation>(),
                    Courses = new List<ProviderCourse> { new ProviderCourse() }
                }
            },
            new ProviderRegistrationDetail
            {
                StatusId = OrganisationStatus.Active,
                ProviderTypeId = ProviderType.Main,
                Provider = new Provider
                {
                    Ukprn = 3,
                    LegalName = "Provider with no courses",
                    Locations = new List<ProviderLocation>
                    {
                        new ProviderLocation
                        {
                            Latitude = 0,
                            Longitude = 0,
                            ProviderCourseLocations = new List<ProviderCourseLocation> 
                            { 
                                new ProviderCourseLocation()
                            }
                        }
                    },
                    Courses = new ()
                }
            },
            new ProviderRegistrationDetail
            {
                StatusId = OrganisationStatus.Removed,
                ProviderTypeId = ProviderType.Main,
                Provider = new Provider
                {
                    Ukprn = 4,
                    LegalName = "Provider main but removed",
                    Locations = new List<ProviderLocation>
                    {
                        new ProviderLocation
                        {
                            Latitude = 0,
                            Longitude = 0,
                            ProviderCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation() }
                        }
                    },
                    Courses = new List<ProviderCourse> { new ProviderCourse() }
                }
            },
            new ProviderRegistrationDetail
            {
                StatusId = OrganisationStatus.Active,
                ProviderTypeId = ProviderType.Supporting,
                Provider = new Provider
                {
                    Ukprn = 5,
                    LegalName = "Provider active but supporting",
                    Locations = new List<ProviderLocation>
                    {
                        new ProviderLocation
                        {
                            Latitude = 0,
                            Longitude = 0,
                            ProviderCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation() }
                        }
                    },
                    Courses = new List<ProviderCourse> { new ProviderCourse() }
                }
            }
        };

        _inMemoryContext?.SeedProviderRegistrationDetails(providerRegistrationDetails);
    }

    [Test]
    public async Task GetActiveAndMainProviderRegistrations_ReturnsOnlyValidProviders()
    {
        var _sut = CreateRepository();

        int expectedProviderUkprn = 1;

        var result = await _sut.GetActiveAndMainProviderRegistrations(CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1), "Only a single valid provider should be returned.");

            var provider = result[0];
            
            Assert.That(
                provider.Ukprn, 
                Is.EqualTo(expectedProviderUkprn), 
                "All returned providers should match the filtering criteria."
            );
        });
    }

    private ProviderRegistrationDetailsReadRepository CreateRepository()
    {
        return new ProviderRegistrationDetailsReadRepository(
            _inMemoryContext,
            Mock.Of<ILogger<ProviderRegistrationDetailsReadRepository>>()
        );
    }

    [TearDown]
    public void TearDown()
    {
        _inMemoryContext?.DisposeAsync();
    }
}
