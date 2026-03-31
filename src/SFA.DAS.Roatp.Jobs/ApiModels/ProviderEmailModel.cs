namespace SFA.DAS.Roatp.Jobs.ApiModels;

public record ProviderEmailModel(string TemplateId, Dictionary<string, string> Tokens);
