using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malefashion.Migrations
{
    public partial class AverageRatingAddedProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Products",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Products");
        }
    }
}
