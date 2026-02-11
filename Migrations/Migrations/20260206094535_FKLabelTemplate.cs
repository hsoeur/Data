using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKLabelTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Usage",
                table: "LabelTemplates",
                newName: "LabelTypeUid");

            migrationBuilder.AlterColumn<string>(
                name: "Product",
                table: "LabelTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LabelTemplates_Product",
                table: "LabelTemplates",
                column: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTemplates_Articles_Product",
                table: "LabelTemplates",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabelTemplates_Articles_Product",
                table: "LabelTemplates");

            migrationBuilder.DropIndex(
                name: "IX_LabelTemplates_Product",
                table: "LabelTemplates");

            migrationBuilder.RenameColumn(
                name: "LabelTypeUid",
                table: "LabelTemplates",
                newName: "Usage");

            migrationBuilder.AlterColumn<string>(
                name: "Product",
                table: "LabelTemplates",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
