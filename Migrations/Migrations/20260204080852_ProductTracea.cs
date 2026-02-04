using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ProductTracea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductStatuses",
                columns: table => new
                {
                    ProductStatusUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStatuses", x => x.ProductStatusUid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductTraceabilities",
                columns: table => new
                {
                    ProductTraceabilityUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductUid = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductSerialNumber = table.Column<int>(type: "int", nullable: false),
                    ProductStatusUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationDate = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ManufacturingOrderUid = table.Column<int>(type: "int", nullable: false),
                    UserUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CycleTime = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    OperationNumber = table.Column<int>(type: "int", nullable: false),
                    MachineUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTraceabilities", x => x.ProductTraceabilityUid);
                    table.ForeignKey(
                        name: "FK_ProductTraceabilities_Machines_MachineUid",
                        column: x => x.MachineUid,
                        principalTable: "Machines",
                        principalColumn: "MachineUid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTraceabilities_ProductStatuses_ProductStatusUid",
                        column: x => x.ProductStatusUid,
                        principalTable: "ProductStatuses",
                        principalColumn: "ProductStatusUid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTraceabilities_Users_UserUid",
                        column: x => x.UserUid,
                        principalTable: "Users",
                        principalColumn: "UserUid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTraceabilities_MachineUid",
                table: "ProductTraceabilities",
                column: "MachineUid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTraceabilities_ProductStatusUid",
                table: "ProductTraceabilities",
                column: "ProductStatusUid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTraceabilities_UserUid",
                table: "ProductTraceabilities",
                column: "UserUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTraceabilities");

            migrationBuilder.DropTable(
                name: "ProductStatuses");
        }
    }
}
