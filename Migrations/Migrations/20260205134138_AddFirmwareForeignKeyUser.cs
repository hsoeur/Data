using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddFirmwareForeignKeyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "Firmwares",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // Supprimer l'index existant si présent pour éviter "Duplicate key name"
            migrationBuilder.Sql(@"
SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Firmwares' AND INDEX_NAME = 'IX_Firmwares_UserUid');
SET @stmt := IF(@exists >0, 'DROP INDEX `IX_Firmwares_UserUid` ON `Firmwares`', 'SELECT 1');
PREPARE s FROM @stmt; EXECUTE s; DEALLOCATE PREPARE s;
");

            // Nettoyer les firmwares avec des UserUid orphelins
            migrationBuilder.Sql(@"
DELETE FROM `Firmwares`
WHERE `UserUid` IS NULL
 OR `UserUid` NOT IN (SELECT `UserUid` FROM `Users`);
");

            migrationBuilder.CreateIndex(
                name: "IX_Firmwares_UserUid",
                table: "Firmwares",
                column: "UserUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Firmwares_Users_UserUid",
                table: "Firmwares",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Firmwares_Users_UserUid",
                table: "Firmwares");

            migrationBuilder.DropIndex(
                name: "IX_Firmwares_UserUid",
                table: "Firmwares");

            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "Firmwares",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
