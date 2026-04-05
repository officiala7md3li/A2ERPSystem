using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainDrivenERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_ProductWarehousePromoCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DiscountGroupId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountExempt",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxExempt",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumDiscountPercent",
                table: "Products",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumSalePrice",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaxGroupId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxGroupSource",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Category");

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DiscountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxUses = table.Column<int>(type: "int", nullable: true),
                    MaxUsesPerCustomer = table.Column<int>(type: "int", nullable: true),
                    MinimumOrderAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsCombinable = table.Column<bool>(type: "bit", nullable: false),
                    CurrentUses = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AcceptsReservations = table.Column<bool>(type: "bit", nullable: false),
                    IsDefaultForSales = table.Column<bool>(type: "bit", nullable: false),
                    IsDefaultForPurchases = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouses_Warehouses_ParentWarehouseId",
                        column: x => x.ParentWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromoCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountApplied = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_PromoCodes_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 35, 59, 201, DateTimeKind.Utc).AddTicks(7389));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 35, 59, 201, DateTimeKind.Utc).AddTicks(7391));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 35, 59, 201, DateTimeKind.Utc).AddTicks(7393));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 35, 59, 201, DateTimeKind.Utc).AddTicks(7394));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 35, 59, 201, DateTimeKind.Utc).AddTicks(7394));

            migrationBuilder.CreateIndex(
                name: "IX_Products_DiscountGroupId",
                table: "Products",
                column: "DiscountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TaxGroupId",
                table: "Products",
                column: "TaxGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitOfMeasureId",
                table: "Products",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code_CompanyId",
                table: "PromoCodes",
                columns: new[] { "Code", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_CompanyId",
                table: "PromoCodes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_InvoiceId",
                table: "PromoCodeUsages",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_PromoCodeId_CustomerId",
                table: "PromoCodeUsages",
                columns: new[] { "PromoCodeId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Code_CompanyId",
                table: "Warehouses",
                columns: new[] { "Code", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CompanyId",
                table: "Warehouses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_ParentWarehouseId",
                table: "Warehouses",
                column: "ParentWarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoCodeUsages");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_Products_DiscountGroupId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TaxGroupId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UnitOfMeasureId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDiscountExempt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsTaxExempt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MaximumDiscountPercent",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MinimumSalePrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TaxGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TaxGroupSource",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 13, 0, 74, DateTimeKind.Utc).AddTicks(8194));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 13, 0, 74, DateTimeKind.Utc).AddTicks(8199));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 13, 0, 74, DateTimeKind.Utc).AddTicks(8200));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 13, 0, 74, DateTimeKind.Utc).AddTicks(8204));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 23, 13, 0, 74, DateTimeKind.Utc).AddTicks(8205));
        }
    }
}
