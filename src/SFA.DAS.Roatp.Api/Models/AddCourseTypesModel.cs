using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.Models;

public class AddCourseTypesModel
{
    public IEnumerable<CourseType> CourseTypes { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
}
