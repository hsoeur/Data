using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFirstSerialNumberType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Normalize existing data: replace empty or null FirstSerialNumber with '0'
            migrationBuilder.Sql(@"UPDATE `ManufacturingOrders` SET `FirstSerialNumber` = '0' WHERE `FirstSerialNumber` IS NULL OR `FirstSerialNumber` = '';");

            // Optionally, strip non-numeric prefixes/suffixes (kept simple here). If your data contains non-numeric values, consider custom logic.

            migrationBuilder.AlterColumn<int>(
                name: "FirstSerialNumber",
                table: "ManufacturingOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstSerialNumber",
                table: "ManufacturingOrders",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
