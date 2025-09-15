using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWC.Challenge.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkFooCustomerToAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f26410f1-a1b1-46ff-8e1e-4c841c88ca9b", "AQAAAAIAAYagAAAAECNB9ZMjRokz/IE4KNbQW6JXoFLOKmJq0bzWUW/04IxnNId5cYoKZldUakZcx12WwQ==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("ef1112d6-3447-49e7-8783-7d18d67cd073"), // Customer.FooId
                column: "UserId",
                value: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001")); // Admin User Id
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7819d0b8-c788-4085-b088-908d42fcce76", "AQAAAAIAAYagAAAAEPQTM4e9Vj6Os9DDpOju6yCLX4mXXq3noOLNxHWYj8nH0zBgj0mANg0JA16HnGNSRg==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("ef1112d6-3447-49e7-8783-7d18d67cd073"), // Customer.FooId
                column: "UserId",
                value: null);
        }
    }
}