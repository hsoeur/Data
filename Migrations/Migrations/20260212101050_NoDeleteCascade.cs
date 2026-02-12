using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class NoDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Firmwares_Articles_Product",
                table: "Firmwares");

            migrationBuilder.DropForeignKey(
                name: "FK_Firmwares_Users_UserUid",
                table: "Firmwares");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Articles_ProductUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_LabelTypes_LabelTypeUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_ManufacturingOrders_ManufacturingOrderUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Users_UserUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelTemplates_Articles_Product",
                table: "LabelTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelTemplates_LabelTypes_LabelTypeUid",
                table: "LabelTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Articles_ProductUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Machines_MachineUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Users_UserUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTraceabilities_Machines_MachineUid",
                table: "ProductTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTraceabilities_ProductStatuses_ProductStatusUid",
                table: "ProductTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTraceabilities_Users_UserUid",
                table: "ProductTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Sequences_Articles_Product",
                table: "Sequences");

            migrationBuilder.DropForeignKey(
                name: "FK_SolderingPrograms_Articles_ProductUid",
                table: "SolderingPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_SolderingPrograms_Users_UserUid",
                table: "SolderingPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Articles_ProductUid",
                table: "Tolerances");

            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Machines_MachineUid",
                table: "Tolerances");

            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Users_UserUid",
                table: "Tolerances");

            migrationBuilder.AddForeignKey(
                name: "FK_Firmwares_Articles_Product",
                table: "Firmwares",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Firmwares_Users_UserUid",
                table: "Firmwares",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Articles_ProductUid",
                table: "Labels",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_LabelTypes_LabelTypeUid",
                table: "Labels",
                column: "LabelTypeUid",
                principalTable: "LabelTypes",
                principalColumn: "LabelTypeUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_ManufacturingOrders_ManufacturingOrderUid",
                table: "Labels",
                column: "ManufacturingOrderUid",
                principalTable: "ManufacturingOrders",
                principalColumn: "ManufacturingOrderUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Users_UserUid",
                table: "Labels",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTemplates_Articles_Product",
                table: "LabelTemplates",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTemplates_LabelTypes_LabelTypeUid",
                table: "LabelTemplates",
                column: "LabelTypeUid",
                principalTable: "LabelTypes",
                principalColumn: "LabelTypeUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementTraceabilities_Articles_ProductUid",
                table: "MeasurementTraceabilities",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementTraceabilities_Machines_MachineUid",
                table: "MeasurementTraceabilities",
                column: "MachineUid",
                principalTable: "Machines",
                principalColumn: "MachineUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementTraceabilities_Users_UserUid",
                table: "MeasurementTraceabilities",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTraceabilities_Machines_MachineUid",
                table: "ProductTraceabilities",
                column: "MachineUid",
                principalTable: "Machines",
                principalColumn: "MachineUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTraceabilities_ProductStatuses_ProductStatusUid",
                table: "ProductTraceabilities",
                column: "ProductStatusUid",
                principalTable: "ProductStatuses",
                principalColumn: "ProductStatusUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTraceabilities_Users_UserUid",
                table: "ProductTraceabilities",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sequences_Articles_Product",
                table: "Sequences",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolderingPrograms_Articles_ProductUid",
                table: "SolderingPrograms",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolderingPrograms_Users_UserUid",
                table: "SolderingPrograms",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Articles_ProductUid",
                table: "Tolerances",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Machines_MachineUid",
                table: "Tolerances",
                column: "MachineUid",
                principalTable: "Machines",
                principalColumn: "MachineUid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Users_UserUid",
                table: "Tolerances",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Firmwares_Articles_Product",
                table: "Firmwares");

            migrationBuilder.DropForeignKey(
                name: "FK_Firmwares_Users_UserUid",
                table: "Firmwares");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Articles_ProductUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_LabelTypes_LabelTypeUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_ManufacturingOrders_ManufacturingOrderUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Users_UserUid",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelTemplates_Articles_Product",
                table: "LabelTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelTemplates_LabelTypes_LabelTypeUid",
                table: "LabelTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Articles_ProductUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Machines_MachineUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementTraceabilities_Users_UserUid",
                table: "MeasurementTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTraceabilities_Machines_MachineUid",
                table: "ProductTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTraceabilities_ProductStatuses_ProductStatusUid",
                table: "ProductTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTraceabilities_Users_UserUid",
                table: "ProductTraceabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Sequences_Articles_Product",
                table: "Sequences");

            migrationBuilder.DropForeignKey(
                name: "FK_SolderingPrograms_Articles_ProductUid",
                table: "SolderingPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_SolderingPrograms_Users_UserUid",
                table: "SolderingPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Articles_ProductUid",
                table: "Tolerances");

            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Machines_MachineUid",
                table: "Tolerances");

            migrationBuilder.DropForeignKey(
                name: "FK_Tolerances_Users_UserUid",
                table: "Tolerances");

            migrationBuilder.AddForeignKey(
                name: "FK_Firmwares_Articles_Product",
                table: "Firmwares",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Firmwares_Users_UserUid",
                table: "Firmwares",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Articles_ProductUid",
                table: "Labels",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_LabelTypes_LabelTypeUid",
                table: "Labels",
                column: "LabelTypeUid",
                principalTable: "LabelTypes",
                principalColumn: "LabelTypeUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_ManufacturingOrders_ManufacturingOrderUid",
                table: "Labels",
                column: "ManufacturingOrderUid",
                principalTable: "ManufacturingOrders",
                principalColumn: "ManufacturingOrderUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Users_UserUid",
                table: "Labels",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTemplates_Articles_Product",
                table: "LabelTemplates",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTemplates_LabelTypes_LabelTypeUid",
                table: "LabelTemplates",
                column: "LabelTypeUid",
                principalTable: "LabelTypes",
                principalColumn: "LabelTypeUid",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTraceabilities_Machines_MachineUid",
                table: "ProductTraceabilities",
                column: "MachineUid",
                principalTable: "Machines",
                principalColumn: "MachineUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTraceabilities_ProductStatuses_ProductStatusUid",
                table: "ProductTraceabilities",
                column: "ProductStatusUid",
                principalTable: "ProductStatuses",
                principalColumn: "ProductStatusUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTraceabilities_Users_UserUid",
                table: "ProductTraceabilities",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sequences_Articles_Product",
                table: "Sequences",
                column: "Product",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Articles_ProductUid",
                table: "Tolerances",
                column: "ProductUid",
                principalTable: "Articles",
                principalColumn: "ArticleUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Machines_MachineUid",
                table: "Tolerances",
                column: "MachineUid",
                principalTable: "Machines",
                principalColumn: "MachineUid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerances_Users_UserUid",
                table: "Tolerances",
                column: "UserUid",
                principalTable: "Users",
                principalColumn: "UserUid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
