using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class ChangeTableFlight2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_DestinationAirporttId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_SourceAirporttId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DestinationAirporttId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "SourceAircraftId",
                table: "Flights");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAirporttId",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAircraftId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_SourceAirporttId",
                table: "Flights",
                column: "SourceAirporttId",
                principalTable: "Airports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_DestinationAircraftId",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_SourceAirporttId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DestinationAircraftId",
                table: "Flights");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAirporttId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationAircraftId",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceAircraftId",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_SourceAirporttId",
                table: "Flights",
                column: "SourceAirporttId",
                principalTable: "Airports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
