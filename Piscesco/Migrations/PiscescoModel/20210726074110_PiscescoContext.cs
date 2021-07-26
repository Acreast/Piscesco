using Microsoft.EntityFrameworkCore.Migrations;

namespace Piscesco.Migrations.PiscescoModel
{
    public partial class PiscescoContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Product",
                type: "decimal(2,0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Product");
        }
    }
}
