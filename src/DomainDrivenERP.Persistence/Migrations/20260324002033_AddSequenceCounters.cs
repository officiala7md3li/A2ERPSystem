using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DomainDrivenERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSequenceCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_ParentHeadCode",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_COAId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ParentHeadCode",
                table: "Accounts");

            migrationBuilder.DeleteData(
                table: "LocalizationSettings",
                keyColumn: "Id",
                keyValue: new Guid("b5daf939-7c28-4e4b-8e4f-3b23c2fb4c66"));

            migrationBuilder.AlterColumn<Guid>(
                name: "COAId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParentHeadCode",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentAccountId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CreditNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PipelineSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TaxOrderSetting = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StackingMode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalLineDiscount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalInvoiceDiscount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalHiddenDiscount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InvoiceHiddenDiscountAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InvoiceHiddenDiscountType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PipelineSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebitNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PipelineSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SequenceCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CounterValue = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceCounters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VendorInvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TaxOrderSetting = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StackingMode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalLineDiscount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalInvoiceDiscount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalHiddenDiscount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InvoiceHiddenDiscountAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InvoiceHiddenDiscountType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PipelineSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLevelDiscounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLevelDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLevelDiscounts_CustomerInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CustomerInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLevelDiscounts_VendorInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "VendorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDiscountOverridden = table.Column<bool>(type: "bit", nullable: false),
                    TotalDiscountAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TaxGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsTaxOverridden = table.Column<bool>(type: "bit", nullable: false),
                    TotalTaxAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HiddenDiscountAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HiddenDiscountType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TaxGroupSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountGroupSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_CreditNotes_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CreditNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_CustomerInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CustomerInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_DebitNotes_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "DebitNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_VendorInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "VendorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineDiscountBreakdowns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineDiscountBreakdowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineDiscountBreakdowns_InvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "InvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineTaxBreakdowns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TaxName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsWithholding = table.Column<bool>(type: "bit", nullable: false),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineTaxBreakdowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineTaxBreakdowns_InvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "InvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Cancelled", "CreatedOnUtc", "ModifiedOnUtc", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), false, new DateTime(2026, 3, 24, 0, 20, 32, 813, DateTimeKind.Utc).AddTicks(8254), null, "Electronics" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), false, new DateTime(2026, 3, 24, 0, 20, 32, 813, DateTimeKind.Utc).AddTicks(8258), null, "Clothing" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), false, new DateTime(2026, 3, 24, 0, 20, 32, 813, DateTimeKind.Utc).AddTicks(8259), null, "Books" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), false, new DateTime(2026, 3, 24, 0, 20, 32, 813, DateTimeKind.Utc).AddTicks(8260), null, "Home & Garden" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), false, new DateTime(2026, 3, 24, 0, 20, 32, 813, DateTimeKind.Utc).AddTicks(8260), null, "Sports & Outdoors" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_HeadCode",
                table: "Accounts",
                column: "HeadCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountId",
                table: "Accounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_CompanyId",
                table: "CreditNotes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_CustomerId",
                table: "CreditNotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_OriginalInvoiceId",
                table: "CreditNotes",
                column: "OriginalInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_SequenceNumber_CompanyId",
                table: "CreditNotes",
                columns: new[] { "SequenceNumber", "CompanyId" },
                unique: true,
                filter: "[SequenceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_Status",
                table: "CreditNotes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_CompanyId",
                table: "CustomerInvoices",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_CustomerId",
                table: "CustomerInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_InvoiceDate",
                table: "CustomerInvoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_SequenceNumber_CompanyId",
                table: "CustomerInvoices",
                columns: new[] { "SequenceNumber", "CompanyId" },
                unique: true,
                filter: "[SequenceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_Status",
                table: "CustomerInvoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_CompanyId",
                table: "DebitNotes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_CustomerId",
                table: "DebitNotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_OriginalInvoiceId",
                table: "DebitNotes",
                column: "OriginalInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_SequenceNumber_CompanyId",
                table: "DebitNotes",
                columns: new[] { "SequenceNumber", "CompanyId" },
                unique: true,
                filter: "[SequenceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_Status",
                table: "DebitNotes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLevelDiscounts_InvoiceId",
                table: "InvoiceLevelDiscounts",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceId",
                table: "InvoiceLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_LineDiscountBreakdowns_InvoiceLineId",
                table: "LineDiscountBreakdowns",
                column: "InvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_LineTaxBreakdowns_InvoiceLineId",
                table: "LineTaxBreakdowns",
                column: "InvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceCounters_Prefix_CompanyId_SequenceDate",
                table: "SequenceCounters",
                columns: new[] { "Prefix", "CompanyId", "SequenceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_CompanyId",
                table: "VendorInvoices",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_InvoiceDate",
                table: "VendorInvoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_SequenceNumber_CompanyId",
                table: "VendorInvoices",
                columns: new[] { "SequenceNumber", "CompanyId" },
                unique: true,
                filter: "[SequenceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_Status",
                table: "VendorInvoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_VendorId",
                table: "VendorInvoices",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_VendorInvoiceNumber_VendorId",
                table: "VendorInvoices",
                columns: new[] { "VendorInvoiceNumber", "VendorId" },
                filter: "[VendorInvoiceNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountId",
                table: "Accounts",
                column: "ParentAccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_COAId",
                table: "Transactions",
                column: "COAId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_COAId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "InvoiceLevelDiscounts");

            migrationBuilder.DropTable(
                name: "LineDiscountBreakdowns");

            migrationBuilder.DropTable(
                name: "LineTaxBreakdowns");

            migrationBuilder.DropTable(
                name: "SequenceCounters");

            migrationBuilder.DropTable(
                name: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "CreditNotes");

            migrationBuilder.DropTable(
                name: "CustomerInvoices");

            migrationBuilder.DropTable(
                name: "DebitNotes");

            migrationBuilder.DropTable(
                name: "VendorInvoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_HeadCode",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ParentAccountId",
                table: "Accounts");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ParentAccountId",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "COAId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ParentHeadCode",
                table: "Accounts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "HeadCode");

            migrationBuilder.InsertData(
                table: "LocalizationSettings",
                columns: new[] { "Id", "AllowUserLanguageSelection", "AutoDetectLanguage", "CacheExpirationMinutes", "CacheTranslations", "Cancelled", "DefaultLanguageCode", "FallbackLanguageCode", "LoadAllLanguagesOnStartup", "ResourceFileFormat", "ResourceFilePath", "ShowLanguageSelector", "UseResourceKeys" },
                values: new object[] { new Guid("b5daf939-7c28-4e4b-8e4f-3b23c2fb4c66"), true, true, 60, true, false, "en", "en", true, "json", "Resources/Translations", true, false });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentHeadCode",
                table: "Accounts",
                column: "ParentHeadCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_ParentHeadCode",
                table: "Accounts",
                column: "ParentHeadCode",
                principalTable: "Accounts",
                principalColumn: "HeadCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_COAId",
                table: "Transactions",
                column: "COAId",
                principalTable: "Accounts",
                principalColumn: "HeadCode");
        }
    }
}
