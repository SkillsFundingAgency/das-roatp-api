namespace SFA.DAS.Roatp.Jobs.Services;

public interface ILoadUkrlpAddressesService
{
    Task<bool> LoadAllProvidersAddresses();
    Task<bool> LoadProvidersAddresses();
}
