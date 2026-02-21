using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MercadosProdutos.Migrations
{
    /// <inheritdoc />
    public partial class MarketsReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "marketReviewStatus",
                table: "Market",
                type: "int",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.CreateTable(
                name: "MarketRequestList",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketReviewStatus = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketRequestList", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MarketRequestList_Market_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Market",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketRequestList");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Market");

            migrationBuilder.DropColumn(
                name: "marketReviewStatus",
                table: "Market");

        }
    }
}
