namespace SFA.DAS.Roatp.Jobs.Services;
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
