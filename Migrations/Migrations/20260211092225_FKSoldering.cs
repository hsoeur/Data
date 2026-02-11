using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKSoldering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "SolderingPrograms",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "SolderingPrograms",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SolderingPrograms_ProductUid",
                table: "SolderingPrograms",
                column: "ProductUid");

            migrationBuilder.CreateIndex(
                name: "IX_SolderingPrograms_UserUid",
                table: "SolderingPrograms",
                column: "UserUid");

            migrationBuilder.AddForeignKey(
                name: "FK_SolderingPrograms_Articles_ProductUid",
                table: "SolderingPrograms",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SolderingPrograms_Users_UserUid",
                table: "SolderingPrograms",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolderingPrograms_Articles_ProductUid",
                table: "SolderingPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_SolderingPrograms_Users_UserUid",
                table: "SolderingPrograms");

            migrationBuilder.DropIndex(
                name: "IX_SolderingPrograms_ProductUid",
                table: "SolderingPrograms");

            migrationBuilder.DropIndex(
                name: "IX_SolderingPrograms_UserUid",
                table: "SolderingPrograms");

            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "SolderingPrograms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "SolderingPrograms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
