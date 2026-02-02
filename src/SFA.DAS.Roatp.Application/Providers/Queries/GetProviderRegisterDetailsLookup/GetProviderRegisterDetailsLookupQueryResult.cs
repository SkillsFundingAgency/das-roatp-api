using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderRegisterDetailsLookup;

public class GetProviderRegisterDetailsLookupQueryResult
{
    public int Ukprn { get; set; }
    public OrganisationStatus Status { get; set; }
    public ProviderType Type { get; set; }
    public List<CourseTypeModel> CourseTypes { get; set; } = new();

    public static implicit operator GetProviderRegisterDetailsLookupQueryResult(ProviderRegistrationDetail source)
    {
        if (source == null) return null;
        var result = new GetProviderRegisterDetailsLookupQueryResult
        {
            Ukprn = source.Ukprn,
            Status = (OrganisationStatus)source.StatusId,
            Type = (ProviderType)source.ProviderTypeId,
        };
        foreach (var providerCourse in source?.Provider?.Courses)
        {
            var courseTypeModel = new CourseTypeModel
            {
                CourseType = providerCourse.Standard.CourseType,
            };
            courseTypeModel.Courses.Add(new CourseModel
            {
                LarsCode = providerCourse.LarsCode,
                EffectiveFrom = providerCourse.CreatedDate is DateTime dt
                    ? DateOnly.FromDateTime(dt) : null,
                //EffectiveTo = providerCourse.EffectiveTo
            });
            result.CourseTypes.Add(courseTypeModel);
        }
        return result;
    }
}
public class CourseTypeModel
{
    public CourseType CourseType { get; set; }
    public List<CourseModel> Courses { get; set; } = new();
}

public class CourseModel
{
    public string LarsCode { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}