using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddFirmwareForeingKeyProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Product",
                table: "Firmwares",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // Supprime l'index existant si présent pour éviter l'erreur "Duplicate key name"
            migrationBuilder.Sql(@"
                SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Firmwares' AND INDEX_NAME = 'IX_Firmwares_Product');
                SET @stmt := IF(@exists >0, 'DROP INDEX `IX_Firmwares_Product` ON `Firmwares`', 'SELECT 1');
                PREPARE s FROM @stmt; EXECUTE s; DEALLOCATE PREPARE s;
            ");

            // Nettoyage des enregistrements orphelins pour respecter la contrainte de clé étrangère
            migrationBuilder.Sql(@"
                DELETE FROM `Firmwares`
                WHERE `Product` IS NULL
                OR `Product` NOT IN (SELECT `ArticleUid` FROM `Articles`);
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Firmwares_Product",
                table: "Firmwares",
                column: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Firmwares_Articles_Product",
                table: "Firmwares",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Firmwares_Articles_Product",
                table: "Firmwares");

            migrationBuilder.DropIndex(
                name: "IX_Firmwares_Product",
                table: "Firmwares");

            migrationBuilder.AlterColumn<string>(
                name: "Product",
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
