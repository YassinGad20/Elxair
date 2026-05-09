using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elxair.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "Method",
                table: "Payments",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Payments",
                newName: "Method");

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
