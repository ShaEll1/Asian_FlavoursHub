using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asian_FlavoursHub.Data.Migrations
{
    public partial class changefood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Food",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Food");
        }
    }
}
