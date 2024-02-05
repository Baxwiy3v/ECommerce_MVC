using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malefashion.Migrations
{
    public partial class SlidesUpdateProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Slides_SubTitle",
                table: "Slides");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Slides_SubTitle",
                table: "Slides",
                column: "SubTitle",
                unique: true);
        }
    }
}
