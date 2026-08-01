using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elxair.Migrations
{
    /// <inheritdoc />
    public partial class AddCostPerBottleToPerfumeSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostPerBottle",
                table: "PerfumeSizes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPerBottle",
                table: "PerfumeSizes");
        }
    }
}
