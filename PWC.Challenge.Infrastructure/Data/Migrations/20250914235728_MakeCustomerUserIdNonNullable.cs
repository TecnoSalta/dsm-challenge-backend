using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWC.Challenge.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeCustomerUserIdNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3021c494-e085-4ac3-ae1c-685c45745ab3", "AQAAAAIAAYagAAAAEK8DhuU72y6cGNKphj45XEy1Q4Qh8mXZ9Eh0Frwl7tUfR7dl49xXGvKAnfDa2UGfmw==" });

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Customers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a0b0c0d0-e0f0-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f26410f1-a1b1-46ff-8e1e-4c841c88ca9b", "AQAAAAIAAYagAAAAECNB9ZMjRokz/IE4KNbQW6JXoFLOKmJq0bzWUW/04IxnNId5cYoKZldUakZcx12WwQ==" });

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Customers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}