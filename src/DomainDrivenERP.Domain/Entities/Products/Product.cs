using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenERP.Domain.Entities.Products.DomainEvents;
using DomainDrivenERP.Domain.Enums;
using DomainDrivenERP.Domain.Errors;
using DomainDrivenERP.Domain.Primitives;
using DomainDrivenERP.Domain.Shared.Guards;
using DomainDrivenERP.Domain.Shared.Results;
using DomainDrivenERP.Domain.ValueObjects;

namespace DomainDrivenERP.Domain.Entities.Products;
public class Product : AggregateRoot, IAuditableEntity
{
    public Product() { }
    private Product(Guid id, string name, Price price, int stockQuantity, SKU sku, string model, string details, Guid categoryId, Guid unitOfMeasureId)
               : base(id)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.NumberNegativeOrZero(price.Amount, nameof(price));
        Guard.Against.NumberNegative(stockQuantity, nameof(stockQuantity));
        Guard.Against.Null(sku, nameof(sku));
        Guard.Against.NullOrEmpty(model, nameof(model));
        Guard.Against.NullOrEmpty(details, nameof(details));
        Guard.Against.Null(categoryId, nameof(categoryId));

        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
        SKU = sku;
        Model = model;
        Details = details;
        CategoryId = categoryId;
        UnitOfMeasureId = unitOfMeasureId;
        TaxGroupSource = TaxGroupSource.Category;
    }

    public string Name { get; private set; }
    public Price Price { get; private set; }
    public int StockQuantity { get; private set; }
    public SKU SKU { get; private set; }
    public string Model { get; private set; }
    public string Details { get; private set; }
    public Guid CategoryId { get; private set; }

    // ── Phase 3 Enrichment Fields ──────────────────────────
    public Guid UnitOfMeasureId { get; private set; }               // Required — Phase 1 link
    public Guid? TaxGroupId { get; private set; }                   // Custom tax group — Phase 2
    public TaxGroupSource TaxGroupSource { get; private set; }      // Category / Custom / Exempt
    public bool IsTaxExempt { get; private set; }                   // Fully exempt from all taxes
    public Guid? DiscountGroupId { get; private set; }              // Custom discount group — Phase 2
    public bool IsDiscountExempt { get; private set; }              // No discounts allowed
    public decimal? MinimumSalePrice { get; private set; }          // Cannot sell below this
    public decimal? MaximumDiscountPercent { get; private set; }    // Item-level discount cap

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Result<Product> Create(string name, decimal amount, string currency, int stockQuantity, string sku, string model, string details, Guid categoryId, Guid unitOfMeasureId = default)
    {
        var id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidProductName);
        }

        if (amount <= 0)
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidProductPrice);
        }
        Result<Price> priceResult = Price.Create(amount, currency);
        if (priceResult.IsFailure)
        {
            return Result.Failure<Product>(priceResult.Error);
        }


        if (stockQuantity < 0)
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidStockQuantity);
        }

        Result<SKU> skuResult = SKU.Create(sku);
        if (skuResult.IsFailure)
        {
            return Result.Failure<Product>(skuResult.Error);
        }
        if (string.IsNullOrWhiteSpace(model))
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidModel);
        }

        if (string.IsNullOrWhiteSpace(details))
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidDetails);
        }

        if (categoryId == Guid.Empty)
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidCategoryId);
        }

        var product = new Product(id, name, priceResult.Value, stockQuantity, skuResult.Value, model, details, categoryId, unitOfMeasureId);
        product.RaiseDomainEvent(new CreateProductDomainEvent(product.Id, name, amount, currency, stockQuantity, sku, model, details, categoryId));
        return Result.Success(product);
    }
    public Result<Product> UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidProductName);
        }

        Name = newName;
        RaiseDomainEvent(new UpdateProductNameDomainEvent(Id, newName));
        return Result.Success(this);
    }

    public Result<Product> UpdatePrice(decimal newPriceAmount, string currency)
    {
        if (newPriceAmount <= 0)
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidProductPrice);
        }

        Result<Price> priceResult = Price.Create(newPriceAmount, currency);
        if (priceResult.IsFailure)
        {
            return Result.Failure<Product>(priceResult.Error);
        }
        Price = priceResult.Value;
        RaiseDomainEvent(new UpdateProductPriceDomainEvent(Id, newPriceAmount, currency));
        return Result.Success(this);
    }
    public bool IsInStock(int quantity = 1)
    {
        return StockQuantity >= quantity;
    }

    public Result<Product> DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidStockQuantity);
        }

        if (!IsInStock(quantity))
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InsufficientStock);
        }

        StockQuantity -= quantity;
        return Result.Success(this);
    }

    public Result<Product> IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure<Product>(DomainErrors.ProductErrors.InvalidStockQuantity);
        }

        StockQuantity += quantity;
        return Result.Success(this);
    }
    public Result<decimal> ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
        {
            return Result.Failure<decimal>(DomainErrors.ProductErrors.InvalidDiscountPercentage);
        }

        decimal discountFactor = discountPercentage / 100;
        decimal discountedPrice = Price.Amount - Price.Amount * discountFactor;

        return Result.Success(discountedPrice);
    }

    // ── Phase 3 Domain Methods ────────────────────────────
    /// <summary>
    /// Resolves the effective TaxGroupId based on TaxGroupSource.
    /// Used by Orchestrator during invoice line resolution.
    /// </summary>
    public Guid? GetEffectiveTaxGroupId(Guid? categoryDefaultTaxGroupId) => TaxGroupSource switch
    {
        TaxGroupSource.Exempt  => null,
        TaxGroupSource.Custom  => TaxGroupId,
        TaxGroupSource.Category => categoryDefaultTaxGroupId,
        _ => categoryDefaultTaxGroupId
    };

    public void SetTaxGroup(Guid? taxGroupId, TaxGroupSource source)
    {
        TaxGroupId = taxGroupId;
        TaxGroupSource = source;
        IsTaxExempt = source == TaxGroupSource.Exempt;
    }

    public void SetDiscountGroup(Guid? discountGroupId, bool isExempt = false)
    {
        DiscountGroupId = discountGroupId;
        IsDiscountExempt = isExempt;
    }

    public void SetPricingLimits(decimal? minimumSalePrice, decimal? maximumDiscountPercent)
    {
        MinimumSalePrice = minimumSalePrice;
        MaximumDiscountPercent = maximumDiscountPercent;
    }

    public void SetUnitOfMeasure(Guid unitOfMeasureId)
        => UnitOfMeasureId = unitOfMeasureId;
}
