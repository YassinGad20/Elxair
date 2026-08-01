using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elxair.Migrations
{
    /// <inheritdoc />
    public partial class AddSoldInSeasonToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoldInSeason",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldInSeason",
                table: "OrderItems");
        }
    }
}
