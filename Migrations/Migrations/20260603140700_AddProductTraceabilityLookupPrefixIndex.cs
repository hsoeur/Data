using DataLib;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260603140700_AddProductTraceabilityLookupPrefixIndex")]
    public partial class AddProductTraceabilityLookupPrefixIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE INDEX IX_ProductTraceabilities_ProductUid_OperationNumber ON ProductTraceabilities (ProductUid(255), OperationNumber);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IX_ProductTraceabilities_ProductUid_OperationNumber ON ProductTraceabilities;");
        }
    }
}
