using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKOfLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Labels_ManufacturingOrderUid",
                table: "Labels",
                column: "ManufacturingOrderUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_ManufacturingOrders_ManufacturingOrderUid",
                table: "Labels",
                column: "ManufacturingOrderUid",
                principalTable: "ManufacturingOrders",
                principalColumn: "ManufacturingOrderUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_ManufacturingOrders_ManufacturingOrderUid",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_ManufacturingOrderUid",
                table: "Labels");
        }
    }
}
