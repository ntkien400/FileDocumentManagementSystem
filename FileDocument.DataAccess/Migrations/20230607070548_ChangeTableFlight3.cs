using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class ChangeTableFlight3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_DestinationAircraftId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DestinationAircraftId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "DestinationAircraftId",
                table: "Flights");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAirporttId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DestinationAirporttId",
                table: "Flights",
                column: "DestinationAirporttId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_DestinationAirporttId",
                table: "Flights",
                column: "DestinationAirporttId",
                principalTable: "Airports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_DestinationAirporttId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DestinationAirporttId",
                table: "Flights");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAirporttId",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "DestinationAircraftId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DestinationAircraftId",
                table: "Flights",
                column: "DestinationAircraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_DestinationAircraftId",
                table: "Flights",
                column: "DestinationAircraftId",
                principalTable: "Airports",
                principalColumn: "Id");
        }
    }
}
