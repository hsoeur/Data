using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ManufacturingOrderMeasTracea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManufacturingOrderUid",
                table: "MeasurementTraceabilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_ManufacturingOrderUid",
                table: "MeasurementTraceabilities",
                column: "ManufacturingOrderUid");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementTraceabilities_ManufacturingOrders_ManufacturingO~",
                table: "MeasurementTraceabilities",
                column: "ManufacturingOrderUid",
                principalTable: "ManufacturingOrders",
                principalColumn: "ManufacturingOrderUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_ManufacturingOrders_ManufacturingO~",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementTraceabilities_ManufacturingOrderUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropColumn(
                name: "ManufacturingOrderUid",
                table: "MeasurementTraceabilities");
        }
    }
}
