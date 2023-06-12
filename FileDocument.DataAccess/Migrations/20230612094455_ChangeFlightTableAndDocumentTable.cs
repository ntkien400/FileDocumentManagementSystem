using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class ChangeFlightTableAndDocumentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Report",
                table: "Documents");

            migrationBuilder.AddColumn<bool>(
                name: "IsDocumentReported",
                table: "Flights",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDocumentReported",
                table: "Flights");

            migrationBuilder.AddColumn<bool>(
                name: "Report",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
