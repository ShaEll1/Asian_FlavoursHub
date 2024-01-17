using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asian_FlavoursHub.Data.Migrations
{
    public partial class addingcategorytoshoppingcart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartId",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_ShoppingCartId",
                table: "Category",
                column: "ShoppingCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_ShoppingCart_ShoppingCartId",
                table: "Category",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_ShoppingCart_ShoppingCartId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_ShoppingCartId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId",
                table: "Category");
        }
    }
}
