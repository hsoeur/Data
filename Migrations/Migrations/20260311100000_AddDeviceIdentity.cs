using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
              name: "DeviceIdentities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EquipSN = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
           .Annotation("MySql:CharSet", "utf8mb4"),
                    MacAddressValue = table.Column<string>(type: "char(17)", maxLength: 17, nullable: false)
          .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                 constraints: table =>
                     {
                         table.PrimaryKey("PK_DeviceIdentities", x => x.Id);
                     })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
             name: "IX_DeviceIdentities_EquipSN",
        table: "DeviceIdentities",
             column: "EquipSN",
         unique: true);

            migrationBuilder.CreateIndex(
         name: "IX_DeviceIdentities_MacAddressValue",
          table: "DeviceIdentities",
                column: "MacAddressValue",
         unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "DeviceIdentities");
        }
    }
}
