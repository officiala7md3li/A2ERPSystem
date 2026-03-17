using System.Text;
using DomainDrivenERP.Web.Configuration;
using DomainDrivenERP.Web.Models.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DomainDrivenERP.Web.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _apiSettings = apiSettings.Value;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_apiSettings.Timeout);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            _logger.LogInformation("Making GET request to: {BaseUrl}/{Endpoint}", _httpClient.BaseAddress, endpoint);
            _logger.LogInformation("Authorization header: {Auth}", _httpClient.DefaultRequestHeaders.Authorization?.ToString() ?? "None");

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

            return await ProcessResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while making GET request to {Endpoint}", endpoint);
            return ApiResponse<T>.Failure("An error occurred while processing your request.");
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Making POST request to: {BaseUrl}/{Endpoint}", _httpClient.BaseAddress, endpoint);
            _logger.LogInformation("Request payload: {Json}", json);

            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

            _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

            return await ProcessResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while making POST request to {Endpoint}", endpoint);
            return ApiResponse<T>.Failure("An error occurred while processing your request.");
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);
            return await ProcessResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while making PUT request to {Endpoint}", endpoint);
            return ApiResponse<T>.Failure("An error occurred while processing your request.");
        }
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
            return await ProcessResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while making DELETE request to {Endpoint}", endpoint);
            return ApiResponse<T>.Failure("An error occurred while processing your request.");
        }
    }

    public void SetAuthorizationToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthorizationToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("API Response Content: {Content}", content);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                // Try to deserialize as BaseResponse first
                BaseResponse<object>? baseResponse = JsonConvert.DeserializeObject<BaseResponse<object>>(content);

                if (baseResponse != null && baseResponse.Succeeded)
                {
                    // Handle different response formats
                    if (baseResponse.Data != null)
                    {
                        // Check if it's a list response with Count and Items
                        string dataJson = JsonConvert.SerializeObject(baseResponse.Data);
                        dynamic? dataObject = JsonConvert.DeserializeObject<dynamic>(dataJson);

                        if (dataObject != null && dataObject.Items != null)
                        {
                            // It's a list response, extract the Items
                            dynamic itemsJson = JsonConvert.SerializeObject(dataObject.Items);
                            T? data = JsonConvert.DeserializeObject<T>(itemsJson);
                            return ApiResponse<T>.Success(data);
                        }
                        else
                        {
                            // It's a single object response, use the Data directly
                            T? data = JsonConvert.DeserializeObject<T>(dataJson);
                            return ApiResponse<T>.Success(data);
                        }
                    }
                    else
                    {
                        return ApiResponse<T>.Success(default);
                    }
                }
                else
                {
                    // BaseResponse indicates failure
                    List<string> errors = baseResponse?.Errors ?? new List<string> { baseResponse?.Message ?? "Unknown error" };
                    return ApiResponse<T>.Failure(errors);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing response: {Content}", content);
                return ApiResponse<T>.Failure("Invalid response format.");
            }
        }
        else
        {
            _logger.LogWarning("API request failed with status {StatusCode}: {Content}",
                response.StatusCode, content);
            return ApiResponse<T>.Failure($"Request failed: {response.StatusCode}");
        }
    }
}
