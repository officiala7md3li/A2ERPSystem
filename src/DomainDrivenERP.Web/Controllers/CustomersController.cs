using DomainDrivenERP.Web.Models.Customers;
using DomainDrivenERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IApiService apiService, IAuthService authService, ILogger<CustomersController> logger)
    {
        _apiService = apiService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        await SetAuthorizationToken();

        try
        {
            _logger.LogInformation("Attempting to load customers from API");
            Models.Common.ApiResponse<List<Customer>> response = await _apiService.GetAsync<List<Customer>>("api/v1/customers");

            if (response.IsSuccess && response.Data != null)
            {
                return View(response.Data);
            }
            else
            {
                _logger.LogWarning("Failed to load customers from API: {Error}", response.ErrorMessage);
                TempData["ErrorMessage"] = $"Failed to load customers: {response.ErrorMessage}";
                return View(new List<Customer>());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customers");
            return View(new List<Customer>());
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        await SetAuthorizationToken();

        Models.Common.ApiResponse<Customer> response = await _apiService.GetAsync<Customer>($"api/v1/customers/{id}");

        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(response.Data);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await SetAuthorizationToken();

        var command = new CreateCustomerCommand
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone
        };

        _logger.LogInformation("Attempting to create customer with email: {Email}", model.Email);
        Models.Common.ApiResponse<Customer> response = await _apiService.PostAsync<Customer>("api/v1/customers/create", command);
        _logger.LogInformation("API response - Success: {IsSuccess}, Error: {Error}", response.IsSuccess, response.ErrorMessage);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Customer created successfully!";
            return RedirectToAction(nameof(Index));
        }

        foreach (string error in response.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    private async Task SetAuthorizationToken()
    {
        string? token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _apiService.SetAuthorizationToken(token);
        }
    }
}
