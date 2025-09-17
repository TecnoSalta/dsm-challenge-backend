using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWC.Challenge.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class stepper2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c0ce49fe-1c1d-4b76-897f-c33410962ddb", "AQAAAAIAAYagAAAAEGxvuNJULUHrd9gCohXtuCSdbTnE27DOlDzZC9MC4z8a4eqlXifzCNV4N0s6dn8jRA==" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Dni",
                table: "Customers",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_LicensePlate",
                table: "Cars",
                column: "LicensePlate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Dni",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Cars_LicensePlate",
                table: "Cars");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e779d99d-c08e-41ba-976a-b9a09f3f9079", "AQAAAAIAAYagAAAAEM6+3MnRwMmnHRVG/9FRVZKrvd+DoSU77Zm0f47xacjznwqWlWxNVw/Zi9OA4Al3jg==" });
        }
    }
}
