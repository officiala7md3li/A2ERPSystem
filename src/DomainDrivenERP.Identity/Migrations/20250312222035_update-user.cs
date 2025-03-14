using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainDrivenERP.Identity.Migrations
{
    /// <inheritdoc />
    public partial class updateuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DoB",
                schema: "Security",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                schema: "Security",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Security",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Security",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePic",
                schema: "Security",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "Security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "Code", "ConcurrencyStamp", "DoB", "Gender", "IsActive", "IsDeleted", "PasswordHash", "ProfilePic", "SecurityStamp" },
                values: new object[] { "ADM-1", "82f820ef-9ced-4777-b03b-77565e465561", new DateTime(1998, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, false, "AQAAAAIAAYagAAAAEGUHz6g3IX5oXMH+AnBR76JrRrSteZ/Lj6PhKiipwb51J0j/h2cmfYMdZDirRtG/Xg==", null, "2db9ee0f-7ae3-4e63-9982-396d633f7ac2" });

            migrationBuilder.UpdateData(
                schema: "Security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "Code", "ConcurrencyStamp", "DoB", "Gender", "IsActive", "IsDeleted", "PasswordHash", "ProfilePic", "SecurityStamp" },
                values: new object[] { "USR-1", "98f416ad-6b64-422f-b2b1-be6df3279534", new DateTime(1998, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, false, "AQAAAAIAAYagAAAAEJRer7eJ/pZm0goNcz/iai2c3Q3KRrM70oUYlLDhRFvN5T2iTrYa19GITCrmKgU4zg==", null, "7bf80340-a8c9-4d3f-b4cf-67d854a2d44a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoB",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePic",
                schema: "Security",
                table: "Users");

            migrationBuilder.UpdateData(
                schema: "Security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "Code", "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { null, "da999ccb-4261-414c-b352-08c1b90ac854", "AQAAAAIAAYagAAAAEBWMfFSmSq3pbybByMiJtV+lG6G1uAKIpGvoedsRabjH8mJdowfK4Lq7+PN+MT0Kdw==", "2eb543a5-8279-4295-8113-c210c4d8b1f7" });

            migrationBuilder.UpdateData(
                schema: "Security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "Code", "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { null, "b8a18ae5-b100-4aa9-8b03-ae8c40256d4c", "AQAAAAIAAYagAAAAEC+2NB6h7fQEGhdz5AcVkgWwMqannj+YOOshqehxQsoFmwqbAzNwZPwEmDzlDrPI2w==", "fe88ba60-49bc-4ee1-8ee5-f075402315b0" });
        }
    }
}
