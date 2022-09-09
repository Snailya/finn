using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FINN.API.Migrations
{
    public partial class AddOriginToRequestLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Logs",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Logs");
        }
    }
}
