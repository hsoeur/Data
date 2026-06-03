using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddMeasurementTraceability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementTraceabilities",
                columns: table => new
                {
                    MeasurementTraceabilityUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductTraceabilityUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ToleranceUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Classification = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementTraceabilities", x => x.MeasurementTraceabilityUid);
                    table.ForeignKey(
                        name: "FK_MeasurementTraceabilities_ProductTraceabilities_ProductTrace~",
                        column: x => x.ProductTraceabilityUid,
                        principalTable: "ProductTraceabilities",
                        principalColumn: "ProductTraceabilityUid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasurementTraceabilities_Tolerances_ToleranceUid",
                        column: x => x.ToleranceUid,
                        principalTable: "Tolerances",
                        principalColumn: "ToleranceUid",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_ProductTraceabilityUid_Classificat~",
                table: "MeasurementTraceabilities",
                columns: new[] { "ProductTraceabilityUid", "Classification" });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_ProductTraceabilityUid_ToleranceUid",
                table: "MeasurementTraceabilities",
                columns: new[] { "ProductTraceabilityUid", "ToleranceUid" });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_ToleranceUid_Classification",
                table: "MeasurementTraceabilities",
                columns: new[] { "ToleranceUid", "Classification" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementTraceabilities");
        }
    }
}
