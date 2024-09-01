using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vpngenie.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromocodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PromocodeId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Promocodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Usages = table.Column<int>(type: "integer", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BonusAmount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promocodes", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Promocodes_PromocodeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Promocodes");

            migrationBuilder.DropIndex(
                name: "IX_Users_PromocodeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PromocodeId",
                table: "Users");
        }
    }
}
