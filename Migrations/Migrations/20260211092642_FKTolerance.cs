using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKTolerance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "Tolerances",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tolerances_ProductUid",
                table: "Tolerances",
                column: "ProductUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Articles_ProductUid",
                table: "Tolerances",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Articles_ProductUid",
                table: "Tolerances");

            migrationBuilder.DropIndex(
                name: "IX_Tolerances_ProductUid",
                table: "Tolerances");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "Tolerances",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
