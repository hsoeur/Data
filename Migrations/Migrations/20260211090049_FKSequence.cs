using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Product",
                table: "Sequences",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Sequences_Product",
                table: "Sequences",
                column: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Sequences_Articles_Product",
                table: "Sequences",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sequences_Articles_Product",
                table: "Sequences");

            migrationBuilder.DropIndex(
                name: "IX_Sequences_Product",
                table: "Sequences");

            migrationBuilder.AlterColumn<string>(
                name: "Product",
                table: "Sequences",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
