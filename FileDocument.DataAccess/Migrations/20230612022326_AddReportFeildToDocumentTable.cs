using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class AddReportFeildToDocumentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentRevisions");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Report",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SignatureUrl",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Documents",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UserId",
                table: "Documents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UserId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_UserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Report",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SignatureUrl",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Documents");

            migrationBuilder.CreateTable(
                name: "DocumentRevisions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateRevision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentRevisions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentRevisions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRevisions_DocumentId",
                table: "DocumentRevisions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRevisions_UserId",
                table: "DocumentRevisions",
                column: "UserId");
        }
    }
}
