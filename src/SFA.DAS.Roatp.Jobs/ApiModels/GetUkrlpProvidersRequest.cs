namespace SFA.DAS.Roatp.Jobs.ApiModels;

public record GetUkrlpProvidersRequest(IEnumerable<int> Ukprns, DateTime? UpdatedSinceDate);
