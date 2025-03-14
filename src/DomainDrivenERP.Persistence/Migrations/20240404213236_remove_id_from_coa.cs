using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainDrivenERP.Persistence.Migrations;

/// <inheritdoc />
public partial class remove_id_from_coa : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Id",
            table: "Accounts");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            table: "Accounts",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
    }
}
