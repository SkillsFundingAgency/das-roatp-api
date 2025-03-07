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
                    LegalName = "Provider with a location with no valid ProviderCourseLocations",
                    Locations = new List<ProviderLocation>
                    {
                        new ProviderLocation 
                        {
                            Latitude = 0,
                            Longitude = 0,
                            ProviderCourseLocations = new List<ProviderCourseLocation>() 
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

        var result = await _sut.GetActiveAndMainProviderRegistrations(CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1), "Only a single valid provider should be returned.");
            Assert.That(
                result.All(p => p.StatusId == OrganisationStatus.Active && p.ProviderTypeId == ProviderType.Main), 
                Is.True, 
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
