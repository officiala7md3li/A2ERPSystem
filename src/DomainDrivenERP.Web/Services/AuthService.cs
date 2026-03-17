using DomainDrivenERP.Web.Models.Auth;
using DomainDrivenERP.Web.Models.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace DomainDrivenERP.Web.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IApiService apiService, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
    {
        _apiService = apiService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var loginCommand = new
            {
                request.Email,
                request.Password
            };

            ApiResponse<LoginResponse> response = await _apiService.PostAsync<LoginResponse>("api/v1/auth/Login", loginCommand);
            
            if (response.IsSuccess && response.Data != null)
            {
                await SetTokenAsync(response.Data.Token);
                _apiService.SetAuthorizationToken(response.Data.Token);
                
                // Create claims for cookie authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, response.Data.Id),
                    new Claim(ClaimTypes.Email, response.Data.Email),
                    new Claim(ClaimTypes.Name, response.Data.UserName),
                    new Claim("Token", response.Data.Token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = request.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                };

                await _httpContextAccessor.HttpContext!.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return ApiResponse<LoginResponse>.Failure("An error occurred during login.");
        }
    }

    public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var registerCommand = new
            {
                request.FirstName,
                request.LastName,
                request.Email,
                request.UserName,
                request.Password
            };

            return await _apiService.PostAsync<object>("api/v1/auth/Register", registerCommand);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return ApiResponse<object>.Failure("An error occurred during registration.");
        }
    }

    public async Task LogoutAsync()
    {
        await ClearTokenAsync();
        _apiService.ClearAuthorizationToken();
        
        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        string? token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetTokenAsync()
    {
        if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            return _httpContextAccessor.HttpContext.User.FindFirst("Token")?.Value;
        }
        return null;
    }

    public async Task SetTokenAsync(string token)
    {
        // Token is stored in claims, handled in LoginAsync
        await Task.CompletedTask;
    }

    public async Task ClearTokenAsync()
    {
        // Token is cleared when signing out
        await Task.CompletedTask;
    }
}
