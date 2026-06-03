using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMeasTracea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementTraceabilities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementTraceabilities",
                columns: table => new
                {
                    MeasurementTraceabilityUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MachineUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManufacturingOrderUid = table.Column<int>(type: "int", nullable: true),
                    ProductUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Classification = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationDate = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Label = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OperationNumber = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<int>(type: "int", nullable: false),
                    UnitUid = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementTraceabilities", x => x.MeasurementTraceabilityUid);
                    table.ForeignKey(
                        name: "FK_MeasurementTraceabilities_Articles_ProductUid",
                        column: x => x.ProductUid,
                        principalTable: "Articles",
                        principalColumn: "ArticleUid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasurementTraceabilities_Machines_MachineUid",
                        column: x => x.MachineUid,
                        principalTable: "Machines",
                        principalColumn: "MachineUid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasurementTraceabilities_ManufacturingOrders_ManufacturingO~",
                        column: x => x.ManufacturingOrderUid,
                        principalTable: "ManufacturingOrders",
                        principalColumn: "ManufacturingOrderUid");
                    table.ForeignKey(
                        name: "FK_MeasurementTraceabilities_Users_UserUid",
                        column: x => x.UserUid,
                        principalTable: "Users",
                        principalColumn: "UserUid",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_MachineUid",
                table: "MeasurementTraceabilities",
                column: "MachineUid");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_ManufacturingOrderUid",
                table: "MeasurementTraceabilities",
                column: "ManufacturingOrderUid");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_ProductUid",
                table: "MeasurementTraceabilities",
                column: "ProductUid");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_UserUid",
                table: "MeasurementTraceabilities",
                column: "UserUid");
        }
    }
}
