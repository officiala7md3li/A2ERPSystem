using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainDrivenERP.Persistence.Migrations;

public partial class AddLocalizationTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Languages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                NativeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                IsRTL = table.Column<bool>(type: "bit", nullable: false),
                IsDefault = table.Column<bool>(type: "bit", nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                FlagIcon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                DateFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                TimeFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                NumberFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                CurrencyFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                DisplayOrder = table.Column<int>(type: "int", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Languages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LocalizationSettings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DefaultLanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                AllowUserLanguageSelection = table.Column<bool>(type: "bit", nullable: false),
                AutoDetectLanguage = table.Column<bool>(type: "bit", nullable: false),
                ShowLanguageSelector = table.Column<bool>(type: "bit", nullable: false),
                LoadAllLanguagesOnStartup = table.Column<bool>(type: "bit", nullable: false),
                CacheTranslations = table.Column<bool>(type: "bit", nullable: false),
                CacheExpirationMinutes = table.Column<int>(type: "int", nullable: false),
                FallbackLanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                UseResourceKeys = table.Column<bool>(type: "bit", nullable: false),
                ResourceFileFormat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                ResourceFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LocalizationSettings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TranslationAudits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                ResourceKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TranslationAudits", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TranslationCaches",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CacheKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                JsonContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsValid = table.Column<bool>(type: "bit", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TranslationCaches", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TranslationExports",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LanguageCodes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Modules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ExportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ExportedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                TotalEntries = table.Column<int>(type: "int", nullable: false),
                ExportStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                ErrorLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TranslationExports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TranslationImports",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ImportedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                TotalEntries = table.Column<int>(type: "int", nullable: false),
                SuccessfulEntries = table.Column<int>(type: "int", nullable: false),
                FailedEntries = table.Column<int>(type: "int", nullable: false),
                ImportStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                ErrorLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TranslationImports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LanguageResources",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ResourceKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ResourceValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Group = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                IsHtml = table.Column<bool>(type: "bit", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LanguageResources", x => x.Id);
                table.ForeignKey(
                    name: "FK_LanguageResources_Languages_LanguageId",
                    column: x => x.LanguageId,
                    principalTable: "Languages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_LanguageResources_LanguageId_ResourceKey",
            table: "LanguageResources",
            columns: new[] { "LanguageId", "ResourceKey" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Languages_Code",
            table: "Languages",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Languages_DisplayOrder",
            table: "Languages",
            column: "DisplayOrder");

        migrationBuilder.CreateIndex(
            name: "IX_TranslationAudits_ChangeDate",
            table: "TranslationAudits",
            column: "ChangeDate");

        migrationBuilder.CreateIndex(
            name: "IX_TranslationAudits_LanguageCode_ResourceKey",
            table: "TranslationAudits",
            columns: new[] { "LanguageCode", "ResourceKey" });

        migrationBuilder.CreateIndex(
            name: "IX_TranslationCaches_LanguageCode_CacheKey",
            table: "TranslationCaches",
            columns: new[] { "LanguageCode", "CacheKey" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TranslationExports_ExportDate",
            table: "TranslationExports",
            column: "ExportDate");

        migrationBuilder.CreateIndex(
            name: "IX_TranslationImports_ImportDate",
            table: "TranslationImports",
            column: "ImportDate");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "LanguageResources");
        migrationBuilder.DropTable(name: "Languages");
        migrationBuilder.DropTable(name: "LocalizationSettings");
        migrationBuilder.DropTable(name: "TranslationAudits");
        migrationBuilder.DropTable(name: "TranslationCaches");
        migrationBuilder.DropTable(name: "TranslationExports");
        migrationBuilder.DropTable(name: "TranslationImports");
    }
}