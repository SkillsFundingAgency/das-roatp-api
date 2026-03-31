using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderCoursesReadRepository
{
    Task<ProviderCourse> GetProviderCourse(int providerId, string larsCode);
    Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, string larsCode);
    Task<List<ProviderCourse>> GetAllProviderCourses(int ukprn);
    Task<List<ProviderCourse>> GetShortCoursesAddedOnDate(DateTime dateTime, CancellationToken cancellationToken);
    Task<List<UkprnLarsCodeModel>> GetAllShortCourses(CancellationToken cancellationToken);
}
