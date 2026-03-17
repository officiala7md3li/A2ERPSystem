using DomainDrivenERP.Web.Models.Categories;
using DomainDrivenERP.Web.Models.Products;
using DomainDrivenERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IApiService apiService, IAuthService authService, ILogger<ProductsController> logger)
    {
        _apiService = apiService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        await SetAuthorizationToken();
        
        // For now, return empty list - we'll implement GetAll products later
        var products = new List<Product>();
        return View(products);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        await SetAuthorizationToken();
        
        var response = await _apiService.GetAsync<Product>($"api/v1/products/{id}");
        
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(response.Data);
    }

    public async Task<IActionResult> Create()
    {
        await SetAuthorizationToken();
        await LoadCategoriesAsync();
        return View();
    }

    public async Task<IActionResult> Debug()
    {
        await SetAuthorizationToken();
        await LoadCategoriesAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> TestValidation(AddProductRequest model)
    {
        // Force clear ModelState and manually validate
        ModelState.Clear();

        // Manual validation
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(model.ProductName))
            errors.Add("Product name is required");
        if (string.IsNullOrWhiteSpace(model.Sku))
            errors.Add("SKU is required");
        if (string.IsNullOrWhiteSpace(model.Model))
            errors.Add("Model is required");
        if (model.Amount <= 0)
            errors.Add("Amount must be greater than 0");
        if (string.IsNullOrWhiteSpace(model.Details))
            errors.Add("Details are required");
        if (model.CategoryId == Guid.Empty)
            errors.Add("Category is required");

        if (errors.Count == 0)
        {
            return Json(new { success = true, message = "All validations passed!" });
        }
        else
        {
            return Json(new { success = false, errors = errors, receivedData = model });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddProductRequest model)
    {
        // Debug logging to see what's being received
        _logger.LogInformation("Form submission received:");
        _logger.LogInformation("ProductName: '{ProductName}'", model.ProductName ?? "NULL");
        _logger.LogInformation("SKU: '{Sku}'", model.Sku ?? "NULL");
        _logger.LogInformation("Model: '{Model}'", model.Model ?? "NULL");
        _logger.LogInformation("Amount: {Amount}", model.Amount);
        _logger.LogInformation("Details: '{Details}'", model.Details ?? "NULL");
        _logger.LogInformation("CategoryId: {CategoryId}", model.CategoryId);

        // Log ModelState errors
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid. Errors:");
            foreach (var error in ModelState)
            {
                _logger.LogWarning("Key: {Key}, Errors: {Errors}", error.Key, string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
            }
        }

        if (!ModelState.IsValid)
        {
            await SetAuthorizationToken();
            await LoadCategoriesAsync();
            return View(model);
        }

        await SetAuthorizationToken();

        var command = new
        {
            CategoryId = model.CategoryId,
            ProductId = Guid.NewGuid(),
            ProductName = model.ProductName,
            Amount = model.Amount,
            Currency = model.Currency,
            StockQuantity = model.StockQuantity,
            Sku = model.Sku,
            Model = model.Model,
            Details = model.Details
        };

        var response = await _apiService.PostAsync<Product>("api/v1/products/add", command);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in response.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        await LoadCategoriesAsync();
        return View(model);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        await SetAuthorizationToken();
        
        var response = await _apiService.GetAsync<Product>($"api/v1/products/{id}");
        
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var model = new AddProductRequest
        {
            ProductName = response.Data!.Name,
            Amount = response.Data.Amount,
            Currency = response.Data.Currency,
            StockQuantity = response.Data.StockQuantity,
            Sku = response.Data.Sku,
            Model = response.Data.Model,
            Details = response.Data.Details,
            CategoryId = response.Data.CategoryId
        };

        ViewData["ProductId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AddProductRequest model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = id;
            return View(model);
        }

        await SetAuthorizationToken();

        // Update product name
        var updateNameCommand = new { ProductId = id, NewName = model.ProductName };
        var nameResponse = await _apiService.PutAsync<bool>("api/v1/products/update-name", updateNameCommand);

        // Update product price
        var updatePriceCommand = new { ProductId = id, NewPrice = model.Amount };
        var priceResponse = await _apiService.PutAsync<bool>("api/v1/products/update-price", updatePriceCommand);

        if (nameResponse.IsSuccess && priceResponse.IsSuccess)
        {
            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        var errors = nameResponse.Errors.Concat(priceResponse.Errors);
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        ViewData["ProductId"] = id;
        return View(model);
    }

    private async Task SetAuthorizationToken()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _apiService.SetAuthorizationToken(token);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var response = await _apiService.GetAsync<List<Category>>("api/v1/categories");

            if (response.IsSuccess && response.Data != null && response.Data.Count > 0)
            {
                ViewBag.Categories = response.Data;
            }
            else
            {
                // Fallback to fixed categories with consistent GUIDs for testing
                var fallbackCategories = new List<Category>
                {
                    new Category { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name = "Electronics" },
                    new Category { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name = "Clothing" },
                    new Category { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name = "Books" },
                    new Category { Id = new Guid("44444444-4444-4444-4444-444444444444"), Name = "Home & Garden" },
                    new Category { Id = new Guid("55555555-5555-5555-5555-555555555555"), Name = "Sports & Outdoors" }
                };
                ViewBag.Categories = fallbackCategories;
                _logger.LogWarning("Failed to load categories from API, using fallback categories. Error: {Error}", response?.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories, using fallback categories");
            // Fallback to fixed categories with consistent GUIDs
            var fallbackCategories = new List<Category>
            {
                new Category { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name = "Electronics" },
                new Category { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name = "Clothing" },
                new Category { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name = "Books" },
                new Category { Id = new Guid("44444444-4444-4444-4444-444444444444"), Name = "Home & Garden" },
                new Category { Id = new Guid("55555555-5555-5555-5555-555555555555"), Name = "Sports & Outdoors" }
            };
            ViewBag.Categories = fallbackCategories;
        }
    }
}
