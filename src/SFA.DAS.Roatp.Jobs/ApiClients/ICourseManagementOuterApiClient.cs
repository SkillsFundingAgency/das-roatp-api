namespace SFA.DAS.Roatp.Jobs.ApiClients;

public interface ICourseManagementOuterApiClient
{
    Task<(bool, T)> Get<T>(string uri);
    Task<(bool, U)> Post<T, U>(string uri, T model);
    Task<bool> Post<TModel>(string uri, TModel model);
    Task Delete(string uri, CancellationToken cancellationToken);
}
