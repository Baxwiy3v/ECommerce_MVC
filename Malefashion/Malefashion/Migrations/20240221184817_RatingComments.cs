using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malefashion.Migrations
{
    public partial class RatingComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                table: "Ratings");
        }
    }
}
