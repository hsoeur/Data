using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKArticleLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "Labels",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ProductUid",
                table: "Labels",
                column: "ProductUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Articles_ProductUid",
                table: "Labels",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Articles_ProductUid",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_ProductUid",
                table: "Labels");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "Labels",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
