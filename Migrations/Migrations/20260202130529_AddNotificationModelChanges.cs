using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean invalid numeric strings before altering column types
            migrationBuilder.Sql("UPDATE `ManufacturingOrderNotifications` SET `OldValue` = '0' WHERE `OldValue` = '' OR `OldValue` IS NULL;");
            migrationBuilder.Sql("UPDATE `ManufacturingOrderNotifications` SET `NewValue` = '0' WHERE `NewValue` = '' OR `NewValue` IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "OldValue",
                table: "ManufacturingOrderNotifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "NewValue",
                table: "ManufacturingOrderNotifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OldValue",
                table: "ManufacturingOrderNotifications",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NewValue",
                table: "ManufacturingOrderNotifications",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
