using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PWC.Challenge.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("c0f0a0b0-c0d0-e0f0-0000-000000000001"), null, "Admin", "ADMIN" },
                    { new Guid("c0f0a0b0-c0d0-e0f0-0000-000000000002"), null, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"), 0, "7819d0b8-c788-4085-b088-908d42fcce76", "admin@pwc.challenge.com", true, "Admin", "User", false, null, "ADMIN@PWC.CHALLENGE.COM", "ADMIN@PWC.CHALLENGE.COM", "AQAAAAIAAYagAAAAEPQTM4e9Vj6Os9DDpOju6yCLX4mXXq3noOLNxHWYj8nH0zBgj0mANg0JA16HnGNSRg==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "admin@pwc.challenge.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("c0f0a0b0-c0d0-e0f0-0000-000000000001"), new Guid("a0b0c0d0-e0f0-0000-0000-000000000001") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c0f0a0b0-c0d0-e0f0-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("c0f0a0b0-c0d0-e0f0-0000-000000000001"), new Guid("a0b0c0d0-e0f0-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c0f0a0b0-c0d0-e0f0-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"));
        }
    }
}
