using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DomainDrivenERP.Persistence.Migrations;

/// <inheritdoc />
public partial class SeedLocalization : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Languages",
            columns: new[] { "Id", "Cancelled", "Code", "CurrencyFormat", "DateFormat", "DisplayOrder", "FlagIcon", "IsDefault", "IsEnabled", "IsRTL", "Name", "NativeName", "NumberFormat", "TimeFormat" },
            values: new object[,]
            {
                { new Guid("a4daf939-7c28-4e4b-8e4f-3b23c2fb4c65"), false, "ar", "#,##0.00 ج.م", "dd/MM/yyyy", 2, "🇪🇬", false, true, true, "Arabic", "العربية", "#,##0.00", "HH:mm" },
                { new Guid("c3daf939-7c28-4e4b-8e4f-3b23c2fb4c64"), false, "en", "$#,##0.00", "MM/dd/yyyy", 1, "🇺🇸", true, true, false, "English", "English", "#,##0.00", "hh:mm tt" }
            });

        migrationBuilder.InsertData(
            table: "LocalizationSettings",
            columns: new[] { "Id", "AllowUserLanguageSelection", "AutoDetectLanguage", "CacheExpirationMinutes", "CacheTranslations", "Cancelled", "DefaultLanguageCode", "FallbackLanguageCode", "LoadAllLanguagesOnStartup", "ResourceFileFormat", "ResourceFilePath", "ShowLanguageSelector", "UseResourceKeys" },
            values: new object[] { new Guid("b5daf939-7c28-4e4b-8e4f-3b23c2fb4c66"), true, true, 60, true, false, "en", "en", true, "json", "Resources/Translations", true, false });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Languages",
            keyColumn: "Id",
            keyValue: new Guid("a4daf939-7c28-4e4b-8e4f-3b23c2fb4c65"));

        migrationBuilder.DeleteData(
            table: "Languages",
            keyColumn: "Id",
            keyValue: new Guid("c3daf939-7c28-4e4b-8e4f-3b23c2fb4c64"));

        migrationBuilder.DeleteData(
            table: "LocalizationSettings",
            keyColumn: "Id",
            keyValue: new Guid("b5daf939-7c28-4e4b-8e4f-3b23c2fb4c66"));
    }
}
