using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKLabelTemplate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LabelTypeUid",
                table: "LabelTemplates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LabelTemplates_LabelTypeUid",
                table: "LabelTemplates",
                column: "LabelTypeUid");

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTemplates_LabelTypes_LabelTypeUid",
                table: "LabelTemplates",
                column: "LabelTypeUid",
                principalTable: "LabelTypes",
                principalColumn: "LabelTypeUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabelTemplates_LabelTypes_LabelTypeUid",
                table: "LabelTemplates");

            migrationBuilder.DropIndex(
                name: "IX_LabelTemplates_LabelTypeUid",
                table: "LabelTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "LabelTypeUid",
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
