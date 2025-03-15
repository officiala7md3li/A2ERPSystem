using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DomainDrivenERP.Identity.Migrations;

/// <inheritdoc />
public partial class initialUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Security");

        migrationBuilder.CreateTable(
            name: "Role",
            schema: "Security",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Role", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Users",
            schema: "Security",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ProfilePic = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                Gender = table.Column<int>(type: "int", nullable: false),
                DoB = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

        migrationBuilder.CreateTable(
            name: "RolesClaim",
            schema: "Security",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolesClaim", x => x.Id);
                table.ForeignKey(
                    name: "FK_RolesClaim_Role_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "Security",
                    principalTable: "Role",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UsersClaim",
            schema: "Security",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsersClaim", x => x.Id);
                table.ForeignKey(
                    name: "FK_UsersClaim_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "Security",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UsersLogin",
            schema: "Security",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsersLogin", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_UsersLogin_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "Security",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UsersRole",
            schema: "Security",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsersRole", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_UsersRole_Role_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "Security",
                    principalTable: "Role",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UsersRole_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "Security",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UsersToken",
            schema: "Security",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsersToken", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_UsersToken_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "Security",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            schema: "Security",
            table: "Role",
            columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "cac43a6e-f7bb-4448-baaf-1add431ccbbf", null, "Employee", "EMPLOYEE" },
                { "cbc43a8e-f7bb-4445-baaf-1add431ffbbf", null, "Administrator", "ADMINISTRATOR" }
            });

        migrationBuilder.InsertData(
            schema: "Security",
            table: "Users",
            columns: new[] { "Id", "AccessFailedCount", "Code", "ConcurrencyStamp", "DoB", "Email", "EmailConfirmed", "FirstName", "Gender", "IsActive", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePic", "SecurityStamp", "TwoFactorEnabled", "UserName" },
            values: new object[,]
            {
                { "8e445865-a24d-4543-a6c6-9443d048cdb9", 0, "ADM-1", "eb044431-a81a-477b-bcfa-291371ad0c1a", new DateTime(1998, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@a7md.com", true, "System", 0, false, false, "Admin", false, null, "ADMIN@A7MD.COM", "ADMIN@A7MD.COM", "AQAAAAIAAYagAAAAELNSEYDWtSyyDv3wEjKCtnImS2d1JslLzH/lSA6zP+GxwDaWz94NZs93T/QwRSn0iA==", null, false, null, "03d031c1-a073-4773-97c8-426dd57470e7", false, "admin@a7md.com" },
                { "9e224968-33e4-4652-b7b7-8574d048cdb9", 0, "USR-1", "57dfc496-b0c6-4ddb-996e-25021df971e1", new DateTime(1998, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user@a7md.com", true, "System", 0, false, false, "User", false, null, "USER@A7MD.COM", "USER@A7MD.COM", "AQAAAAIAAYagAAAAEBt2OTO3Y0f1wRNZFWLixuBfWWceaiQSNdK7Ul1nkapaowqmQmkMbnd90m4mFD3anw==", null, false, null, "648cdd0d-5b83-41d3-b752-fb1c3f920edf", false, "user@a7md.com" }
            });

        migrationBuilder.InsertData(
            schema: "Security",
            table: "UsersRole",
            columns: new[] { "RoleId", "UserId" },
            values: new object[,]
            {
                { "cbc43a8e-f7bb-4445-baaf-1add431ffbbf", "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                { "cac43a6e-f7bb-4448-baaf-1add431ccbbf", "9e224968-33e4-4652-b7b7-8574d048cdb9" }
            });

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            schema: "Security",
            table: "Role",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_RolesClaim_RoleId",
            schema: "Security",
            table: "RolesClaim",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            schema: "Security",
            table: "Users",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            schema: "Security",
            table: "Users",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_UsersClaim_UserId",
            schema: "Security",
            table: "UsersClaim",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_UsersLogin_UserId",
            schema: "Security",
            table: "UsersLogin",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_UsersRole_RoleId",
            schema: "Security",
            table: "UsersRole",
            column: "RoleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "RolesClaim",
            schema: "Security");

        migrationBuilder.DropTable(
            name: "UsersClaim",
            schema: "Security");

        migrationBuilder.DropTable(
            name: "UsersLogin",
            schema: "Security");

        migrationBuilder.DropTable(
            name: "UsersRole",
            schema: "Security");

        migrationBuilder.DropTable(
            name: "UsersToken",
            schema: "Security");

        migrationBuilder.DropTable(
            name: "Role",
            schema: "Security");

        migrationBuilder.DropTable(
            name: "Users",
            schema: "Security");
    }
}
