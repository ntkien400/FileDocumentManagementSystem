using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileDocument.DataAccess.Migrations
{
    public partial class ChangeForeignKeyGroupDocTypePermissionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupDocPermissions");

            migrationBuilder.CreateTable(
                name: "GroupDocTypePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDocTypePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupDocTypePermissions_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDocTypePermissions_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDocTypePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupDocTypePermissions_DocumentTypeId",
                table: "GroupDocTypePermissions",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDocTypePermissions_GroupId",
                table: "GroupDocTypePermissions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDocTypePermissions_PermissionId",
                table: "GroupDocTypePermissions",
                column: "PermissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupDocTypePermissions");

            migrationBuilder.CreateTable(
                name: "GroupDocPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDocPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupDocPermissions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDocPermissions_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDocPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupDocPermissions_DocumentId",
                table: "GroupDocPermissions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDocPermissions_GroupId",
                table: "GroupDocPermissions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDocPermissions_PermissionId",
                table: "GroupDocPermissions",
                column: "PermissionId");
        }
    }
}
