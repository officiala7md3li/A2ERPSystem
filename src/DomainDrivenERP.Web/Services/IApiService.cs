using DomainDrivenERP.Web.Models.Common;

namespace DomainDrivenERP.Web.Services;

public interface IApiService
{
    Task<ApiResponse<T>> GetAsync<T>(string endpoint);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint);
    void SetAuthorizationToken(string token);
    void ClearAuthorizationToken();
}
