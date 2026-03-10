namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public record UpsertProviderCourseForecastModel(string TimePeriod, int Quarter, int? EstimatedLearners);
