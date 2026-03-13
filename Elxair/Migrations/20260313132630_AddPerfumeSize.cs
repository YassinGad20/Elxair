using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elxair.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfumeSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Perfumes_PerfumeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Perfumes");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Perfumes");

            migrationBuilder.RenameColumn(
                name: "PerfumeId",
                table: "OrderItems",
                newName: "PerfumeSizeId");

            migrationBuilder.RenameColumn(
                name: "PerfumeId",
                table: "CartItems",
                newName: "PerfumeSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_PerfumeId",
                table: "CartItems",
                newName: "IX_CartItems_PerfumeSizeId");

            migrationBuilder.CreateTable(
                name: "PerfumeSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerfumeId = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfumeSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfumeSizes_Perfumes_PerfumeId",
                        column: x => x.PerfumeId,
                        principalTable: "Perfumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_PerfumeSizeId",
                table: "OrderItems",
                column: "PerfumeSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfumeSizes_PerfumeId",
                table: "PerfumeSizes",
                column: "PerfumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_PerfumeSizes_PerfumeSizeId",
                table: "CartItems",
                column: "PerfumeSizeId",
                principalTable: "PerfumeSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_PerfumeSizes_PerfumeSizeId",
                table: "OrderItems",
                column: "PerfumeSizeId",
                principalTable: "PerfumeSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_PerfumeSizes_PerfumeSizeId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_PerfumeSizes_PerfumeSizeId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "PerfumeSizes");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_PerfumeSizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "PerfumeSizeId",
                table: "OrderItems",
                newName: "PerfumeId");

            migrationBuilder.RenameColumn(
                name: "PerfumeSizeId",
                table: "CartItems",
                newName: "PerfumeId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_PerfumeSizeId",
                table: "CartItems",
                newName: "IX_CartItems_PerfumeId");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Perfumes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Perfumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Perfumes_PerfumeId",
                table: "CartItems",
                column: "PerfumeId",
                principalTable: "Perfumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
