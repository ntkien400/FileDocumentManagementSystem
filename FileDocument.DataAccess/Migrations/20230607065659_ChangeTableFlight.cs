using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class ChangeTableFlight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Aircrafts_DestinationAircraftId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_AirportId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DestinationAircraftId",
                table: "Flights");

            migrationBuilder.RenameColumn(
                name: "AirportId",
                table: "Flights",
                newName: "SourceAirporttId");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_AirportId",
                table: "Flights",
                newName: "IX_Flights_SourceAirporttId");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAircraftId",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AircraftId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationAirporttId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AircraftId",
                table: "Flights",
                column: "AircraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DestinationAirporttId",
                table: "Flights",
                column: "DestinationAirporttId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Aircrafts_AircraftId",
                table: "Flights",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_DestinationAirporttId",
                table: "Flights",
                column: "DestinationAirporttId",
                principalTable: "Airports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_SourceAirporttId",
                table: "Flights",
                column: "SourceAirporttId",
                principalTable: "Airports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Aircrafts_AircraftId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_DestinationAirporttId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_SourceAirporttId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_AircraftId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DestinationAirporttId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "AircraftId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "DestinationAirporttId",
                table: "Flights");

            migrationBuilder.RenameColumn(
                name: "SourceAirporttId",
                table: "Flights",
                newName: "AirportId");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_SourceAirporttId",
                table: "Flights",
                newName: "IX_Flights_AirportId");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAircraftId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DestinationAircraftId",
                table: "Flights",
                column: "DestinationAircraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Aircrafts_DestinationAircraftId",
                table: "Flights",
                column: "DestinationAircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_AirportId",
                table: "Flights",
                column: "AirportId",
                principalTable: "Airports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
