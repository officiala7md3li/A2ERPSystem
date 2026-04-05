using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainDrivenERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_BusinessPartyAndPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 22, 48, 0, 63, DateTimeKind.Utc).AddTicks(7123));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 22, 48, 0, 63, DateTimeKind.Utc).AddTicks(7127));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 22, 48, 0, 63, DateTimeKind.Utc).AddTicks(7128));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 22, 48, 0, 63, DateTimeKind.Utc).AddTicks(7129));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedOnUtc",
                value: new DateTime(2026, 4, 5, 22, 48, 0, 63, DateTimeKind.Utc).AddTicks(7130));
        }
    }
}
