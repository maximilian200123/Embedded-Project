using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarbageCollectionApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateAdminTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarbageCollections_GarbageBins_IdGarbageBin",
                table: "GarbageCollections");

            migrationBuilder.DropIndex(
                name: "IX_GarbageCollections_IdGarbageBin",
                table: "GarbageCollections");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.CreateIndex(
                name: "IX_GarbageCollections_IdGarbageBin",
                table: "GarbageCollections",
                column: "IdGarbageBin");

            migrationBuilder.AddForeignKey(
                name: "FK_GarbageCollections_GarbageBins_IdGarbageBin",
                table: "GarbageCollections",
                column: "IdGarbageBin",
                principalTable: "GarbageBins",
                principalColumn: "IdGarbageBin",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
