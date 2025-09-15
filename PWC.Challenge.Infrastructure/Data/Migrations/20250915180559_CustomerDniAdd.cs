using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWC.Challenge.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomerDniAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dni",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f634f989-3a05-42c8-be1f-602ae643c282", "AQAAAAIAAYagAAAAEBfPvxcnsi8GAoV/WSk0lTCuM5PP2KfzTk3BPTATogh0FM8PbUnGMSiwFI5IeHATcQ==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("ef1112d6-3447-49e7-8783-7d18d67cd073"),
                column: "Dni",
                value: "123456789");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3021c494-e085-4ac3-ae1c-685c45745ab3", "AQAAAAIAAYagAAAAEK8DhuU72y6cGNKphj45XEy1Q4Qh8mXZ9Eh0Frwl7tUfR7dl49xXGvKAnfDa2UGfmw==" });
        }
    }
}
