using System;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries;

public static class TestDataHelper
{
    public static ProviderRegistrationDetail GetProviderRegistrationDetails()
        => new()
        {
            Ukprn = 12345678,
            StatusId = (int)ProviderStatusType.Active,
            ProviderTypeId = (int)ProviderType.Main,
            ProviderCourseTypes =
            [
                new ProviderCourseType
                {
                    CourseType = CourseType.Apprenticeship,
                },
                new ProviderCourseType
                {
                    CourseType = CourseType.ShortCourse
                }
            ],
            Provider = new Provider
            {
                Id = 1,
                ProviderCoursesTimelines =
                [
                    new ProviderCoursesTimeline
                    {
                        LarsCode = "LARS123",
                        EffectiveFrom = new DateTime(2023, 1, 1, 0,0,0, DateTimeKind.Unspecified),
                        EffectiveTo = null,
                        Standard = new Standard { CourseType = CourseType.Apprenticeship }
                    }
                ],
                ProviderAllowedCourses =
                [
                    new ProviderAllowedCourse
                    {
                        LarsCode = "LARS123",
                        Ukprn = 12345678,
                        LastDateStarts = DateTime.Today
                    }
                ],
                Courses =
                [
                    new Domain.Entities.ProviderCourse
                    {
                        LarsCode = "LARS123",
                        ProviderId = 1
                    }
                ]
            }
        };

    public static ProviderRegistrationDetail GetProviderRegistrationDetailsCourseIsNotAllowed()
        => new()
        {
            Ukprn = 12345678,
            StatusId = (int)ProviderStatusType.Active,
            ProviderTypeId = (int)ProviderType.Main,
            ProviderCourseTypes =
            [
                new ProviderCourseType
                {
                    CourseType = CourseType.Apprenticeship,
                },
                new ProviderCourseType
                {
                    CourseType = CourseType.ShortCourse
                }
            ],
            Provider = new Provider
            {
                Id = 1,
                ProviderCoursesTimelines =
                [
                    new ProviderCoursesTimeline
                    {
                        LarsCode = "LARS123",
                        EffectiveFrom = new DateTime(2023, 1, 1, 0,0,0, DateTimeKind.Unspecified),
                        EffectiveTo = null,
                        Standard = new Standard { CourseType = CourseType.Apprenticeship }
                    }
                ],
                Courses =
                [
                    new Domain.Entities.ProviderCourse
                    {
                        LarsCode = "LARS123",
                        ProviderId = 1
                    }
                ]
            }
        };

    public static ProviderRegistrationDetail GetProviderRegistrationDetailsProviderDoesNotProvideCourse()
        => new()
        {
            Ukprn = 12345678,
            StatusId = (int)ProviderStatusType.Active,
            ProviderTypeId = (int)ProviderType.Main,
            ProviderCourseTypes =
            [
                new ProviderCourseType
                {
                    CourseType = CourseType.Apprenticeship,
                },
                new ProviderCourseType
                {
                    CourseType = CourseType.ShortCourse
                }
            ],
            Provider = new Provider
            {
                Id = 1,
                ProviderCoursesTimelines =
                [
                    new ProviderCoursesTimeline
                    {
                        LarsCode = "LARS123",
                        EffectiveFrom = new DateTime(2023, 1, 1, 0,0,0, DateTimeKind.Unspecified),
                        EffectiveTo = null,
                        Standard = new Standard { CourseType = CourseType.Apprenticeship }
                    }
                ],
                ProviderAllowedCourses =
                [
                    new ProviderAllowedCourse
                    {
                        LarsCode = "LARS123",
                        Ukprn = 12345678,
                        LastDateStarts = DateTime.Today
                    }
                ]
            }
        };
}
