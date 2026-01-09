using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Jobs.ApiModels;

public record AllowedCourseType(int CourseTypeId, CourseType CourseType, Domain.Models.LearningType LearningType);
