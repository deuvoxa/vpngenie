using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vpngenie.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Promocodes_PromocodeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PromocodeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PromocodeId",
                table: "Users");

            migrationBuilder.AddColumn<long[]>(
                name: "PromocodeUsages",
                table: "Promocodes",
                type: "bigint[]",
                nullable: false,
                defaultValue: new long[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromocodeUsages",
                table: "Promocodes");

            migrationBuilder.AddColumn<Guid>(
                name: "PromocodeId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PromocodeId",
                table: "Users",
                column: "PromocodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Promocodes_PromocodeId",
                table: "Users",
                column: "PromocodeId",
                principalTable: "Promocodes",
                principalColumn: "Id");
        }
    }
}
