using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MercadosProdutos.Migrations
{
    /// <inheritdoc />
    public partial class ProductDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "ProductList",
                type: "nvarchar(max)",
                nullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_ProductList_MarketID",
                table: "ProductList",
                column: "MarketIdentification");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductList_Market_MarketID",
                table: "ProductList",
                column: "MarketIdentification",
                principalTable: "Market",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductList_Market_MarketID",
                table: "ProductList");

            migrationBuilder.DropIndex(
                name: "IX_ProductList_MarketID",
                table: "ProductList");


            migrationBuilder.DropColumn(
                name: "description",
                table: "ProductList");
        }
    }
}
