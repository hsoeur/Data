using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKArticleLabelUserAndLabelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "Labels",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "LabelTypeUid",
                table: "Labels",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_LabelTypeUid",
                table: "Labels",
                column: "LabelTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_UserUid",
                table: "Labels",
                column: "UserUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_LabelTypes_LabelTypeUid",
                table: "Labels",
                column: "LabelTypeUid",
                principalTable: "LabelTypes",
                principalColumn: "LabelTypeUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Users_UserUid",
                table: "Labels",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_LabelTypes_LabelTypeUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Users_UserUid",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_LabelTypeUid",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_UserUid",
                table: "Labels");

            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "Labels",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "LabelTypeUid",
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
