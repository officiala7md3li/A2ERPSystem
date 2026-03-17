using DomainDrivenERP.Web.Models.Orders;
using DomainDrivenERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IApiService apiService, IAuthService authService, ILogger<OrdersController> logger)
    {
        _apiService = apiService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        await SetAuthorizationToken();
        
        // For now, return empty list - we'll implement GetAll orders later
        var orders = new List<Order>();
        return View(orders);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        await SetAuthorizationToken();
        
        var response = await _apiService.GetAsync<Order>($"api/v1/orders/{id}");
        
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(response.Data);
    }

    public IActionResult Create()
    {
        var model = new CreateOrderRequest();
        model.Items.Add(new OrderItemDto()); // Add one empty item to start
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await SetAuthorizationToken();

        var command = new
        {
            CustomerId = model.CustomerId,
            Items = model.Items.Select(item => new
            {
                ProductId = item.ProductId,
                ProductPrice = item.ProductPrice,
                Quantity = item.Quantity
            }).ToList()
        };

        var response = await _apiService.PostAsync<Order>("api/v1/orders/create", command);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Order created successfully!";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in response.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await SetAuthorizationToken();

        var command = new { OrderId = id };
        var response = await _apiService.PutAsync<bool>("api/v1/orders/cancel", command);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Order cancelled successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = response.ErrorMessage;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(Guid id, OrderStatus newStatus)
    {
        await SetAuthorizationToken();

        var command = new { OrderId = id, NewStatus = newStatus };
        var response = await _apiService.PutAsync<bool>("api/v1/orders/change-status", command);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Order status updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = response.ErrorMessage;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task SetAuthorizationToken()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _apiService.SetAuthorizationToken(token);
        }
    }
}
