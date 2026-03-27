using System;

namespace SFA.DAS.Roatp.Domain.Models;

public record ProviderCourseWithLastForecastDate(int Ukprn, string LarsCode, DateTime UpdatedDate);
