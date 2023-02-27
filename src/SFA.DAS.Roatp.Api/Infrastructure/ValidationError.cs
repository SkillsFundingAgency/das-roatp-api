namespace SFA.DAS.Roatp.Api.Infrastructure;

public class ValidationError
{
    public string PropertyName { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
}