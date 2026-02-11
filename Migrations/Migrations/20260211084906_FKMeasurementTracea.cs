using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FKMeasurementTracea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "MeasurementTraceabilities",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "MeasurementTraceabilities",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MachineUid",
                table: "MeasurementTraceabilities",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "MeasurementTraceabilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_MachineUid",
                table: "MeasurementTraceabilities",
                column: "MachineUid");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_ProductUid",
                table: "MeasurementTraceabilities",
                column: "ProductUid");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementTraceabilities_UserUid",
                table: "MeasurementTraceabilities",
                column: "UserUid");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementTraceabilities_Articles_ProductUid",
                table: "MeasurementTraceabilities",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementTraceabilities_Machines_MachineUid",
                table: "MeasurementTraceabilities",
                column: "MachineUid",
                principalTable: "Machines",
                principalColumn: "MachineUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementTraceabilities_Users_UserUid",
                table: "MeasurementTraceabilities",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Articles_ProductUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Machines_MachineUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Users_UserUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementTraceabilities_MachineUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementTraceabilities_ProductUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementTraceabilities_UserUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "MeasurementTraceabilities");

            migrationBuilder.AlterColumn<string>(
                name: "UserUid",
                table: "MeasurementTraceabilities",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "MeasurementTraceabilities",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MachineUid",
                table: "MeasurementTraceabilities",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
