using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class AddFlightToFlightTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FlightEnd",
                table: "Flights",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlightEnd",
                table: "Flights");
        }
    }
}
