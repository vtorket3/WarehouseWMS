using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchToStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductBatchId",
                table: "StockBalances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_ProductBatchId",
                table: "StockBalances",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_WarehouseId",
                table: "StockBalances",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockBalances_ProductBatches_ProductBatchId",
                table: "StockBalances",
                column: "ProductBatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockBalances_Warehouses_WarehouseId",
                table: "StockBalances",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockBalances_ProductBatches_ProductBatchId",
                table: "StockBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_StockBalances_Warehouses_WarehouseId",
                table: "StockBalances");

            migrationBuilder.DropIndex(
                name: "IX_StockBalances_ProductBatchId",
                table: "StockBalances");

            migrationBuilder.DropIndex(
                name: "IX_StockBalances_WarehouseId",
                table: "StockBalances");

            migrationBuilder.DropColumn(
                name: "ProductBatchId",
                table: "StockBalances");
        }
    }
}
