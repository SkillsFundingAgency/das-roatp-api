namespace SFA.DAS.Roatp.Jobs.Services;

public interface IRefreshProviderDetailsFromUkrlpService
{
    Task RefreshProviderDetailsFromUkrlp(bool recentUpdatesOnly);
}