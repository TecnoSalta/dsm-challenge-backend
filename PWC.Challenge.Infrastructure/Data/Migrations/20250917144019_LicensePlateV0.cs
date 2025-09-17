using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWC.Challenge.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class LicensePlateV0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicensePlate",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e779d99d-c08e-41ba-976a-b9a09f3f9079", "AQAAAAIAAYagAAAAEM6+3MnRwMmnHRVG/9FRVZKrvd+DoSU77Zm0f47xacjznwqWlWxNVw/Zi9OA4Al3jg==" });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "LicensePlate",
                value: "AB753BG");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"),
                column: "LicensePlate",
                value: "AB753BD");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"),
                column: "LicensePlate",
                value: "AB753BY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicensePlate",
                table: "Cars");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f634f989-3a05-42c8-be1f-602ae643c282", "AQAAAAIAAYagAAAAEBfPvxcnsi8GAoV/WSk0lTCuM5PP2KfzTk3BPTATogh0FM8PbUnGMSiwFI5IeHATcQ==" });
        }
    }
}
