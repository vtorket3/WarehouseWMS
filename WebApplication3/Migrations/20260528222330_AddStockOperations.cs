using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class AddStockOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockBalances_ProductBatches_ProductBatchId",
                table: "StockBalances");

            migrationBuilder.DropIndex(
                name: "IX_StockBalances_ProductBatchId",
                table: "StockBalances");

            migrationBuilder.DropColumn(
                name: "ProductBatchId",
                table: "StockBalances");

            migrationBuilder.CreateTable(
                name: "StockOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockOperations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockOperations_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_BatchId",
                table: "StockBalances",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_ProductId",
                table: "ProductBatches",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOperations_ProductId",
                table: "StockOperations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOperations_WarehouseId",
                table: "StockOperations",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBatches_Products_ProductId",
                table: "ProductBatches",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockBalances_ProductBatches_BatchId",
                table: "StockBalances",
                column: "BatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductBatches_Products_ProductId",
                table: "ProductBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_StockBalances_ProductBatches_BatchId",
                table: "StockBalances");

            migrationBuilder.DropTable(
                name: "StockOperations");

            migrationBuilder.DropIndex(
                name: "IX_StockBalances_BatchId",
                table: "StockBalances");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_ProductId",
                table: "ProductBatches");

            migrationBuilder.AddColumn<int>(
                name: "ProductBatchId",
                table: "StockBalances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_ProductBatchId",
                table: "StockBalances",
                column: "ProductBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockBalances_ProductBatches_ProductBatchId",
                table: "StockBalances",
                column: "ProductBatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id");
        }
    }
}
