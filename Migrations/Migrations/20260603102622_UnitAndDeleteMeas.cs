using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UnitAndDeleteMeas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Tolerances");

            migrationBuilder.AddColumn<string>(
                name: "UnitUid",
                table: "Tolerances",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    UnitUid = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.UnitUid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tolerances_UnitUid",
                table: "Tolerances",
                column: "UnitUid");

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Units_UnitUid",
                table: "Tolerances",
                column: "UnitUid",
                principalTable: "Units",
                principalColumn: "UnitUid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Units_UnitUid",
                table: "Tolerances");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Tolerances_UnitUid",
                table: "Tolerances");

            migrationBuilder.DropColumn(
                name: "UnitUid",
                table: "Tolerances");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Tolerances",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
