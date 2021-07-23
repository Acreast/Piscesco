using Microsoft.EntityFrameworkCore.Migrations;

namespace Piscesco.Migrations.PiscescoModel
{
    public partial class AddStallImageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StallImage",
                table: "Stall",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StallImage",
                table: "Stall");
        }
    }
}
