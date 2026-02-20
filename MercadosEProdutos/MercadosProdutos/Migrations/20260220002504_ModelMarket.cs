using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MercadosProdutos.Migrations
{
    /// <inheritdoc />
    public partial class ModelMarket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "MarketAssociated",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_marketAssociated_MarketID",
                table: "MarketAssociated",
                column: "MarketID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_marketAssociatedList_Market_MarketID",
                table: "MarketAssociated");

            migrationBuilder.DropIndex(
                name: "IX_marketAssociatedList_MarketID",
                table: "MarketAssociated");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "MarketAssociated",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
