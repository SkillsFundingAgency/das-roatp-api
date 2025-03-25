using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryResult
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public ShortProviderAddressModel Address { get; set; }
    public ContactModel Contact { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public int LarsCode { get; set; }
    public string IFateReferenceNumber { get; set; }
    public QarModel QAR { get; set; }
    public ReviewModel Reviews { get; set; }
    public IEnumerable<LocationModel> Locations { get; set; } = [];
    public Guid? ShortlistId { get; set; }
}
