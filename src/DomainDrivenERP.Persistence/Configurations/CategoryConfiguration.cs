using DomainDrivenERP.Domain.Entities.Categories;
using DomainDrivenERP.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenERP.Persistence.Configurations;
internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    private static readonly Guid ElectronicsId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ClothingId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid BooksId = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid HomeGardenId = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid SportsOutdoorsId = new("55555555-5555-5555-5555-555555555555");

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable(TableNames.Categories);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasMany(c => c.Products)
         .WithOne()
         .HasForeignKey(o => o.CategoryId);

        // Seed default categories
        builder.HasData(
            CreateCategory(ElectronicsId, "Electronics"),
            CreateCategory(ClothingId, "Clothing"),
            CreateCategory(BooksId, "Books"),
            CreateCategory(HomeGardenId, "Home & Garden"),
            CreateCategory(SportsOutdoorsId, "Sports & Outdoors")
        );
    }

    private static object CreateCategory(Guid id, string name)
    {
        return new
        {
            Id = id,
            Name = name,
            Cancelled = false,
            CreatedOnUtc = DateTime.UtcNow,
            ModifiedOnUtc = (DateTime?)null
        };
    }
}
