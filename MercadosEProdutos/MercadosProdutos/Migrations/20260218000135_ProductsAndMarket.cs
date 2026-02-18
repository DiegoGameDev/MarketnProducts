using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MercadosProdutos.Migrations
{
    /// <inheritdoc />
    public partial class ProductsAndMarket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Market",
            columns: table => new
            {
                ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                marketName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                marketLocal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                phoneOrEmailContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                cnpj = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketList", x => x.ID);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductList",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    productName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarketIdentification = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductList", x => x.ID);
                    table.ForeignKey(
                    name: "FK_ProductList_Market_MarketIdentification",
                    column: x => x.MarketIdentification,
                    principalTable: "Market",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                });

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Market");
            migrationBuilder.DropTable(name: "ProductList");
        }
    }
}
