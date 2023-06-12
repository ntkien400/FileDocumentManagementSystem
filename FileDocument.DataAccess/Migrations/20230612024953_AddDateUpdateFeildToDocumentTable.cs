using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class AddDateUpdateFeildToDocumentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdate",
                table: "Documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUpdate",
                table: "Documents");
        }
    }
}
