using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarbageCollectionApp.Migrations
{
    /// <inheritdoc />
    public partial class AddGarbageStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Citizens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Cnp = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citizens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarbageBins",
                columns: table => new
                {
                    IdGarbageBin = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarbageBins", x => x.IdGarbageBin);
                });

            migrationBuilder.CreateTable(
                name: "GarbageBinCitizens",
                columns: table => new
                {
                    IdGarbageBin = table.Column<string>(type: "TEXT", nullable: false),
                    IdCitizen = table.Column<int>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarbageBinCitizens", x => new { x.IdGarbageBin, x.IdCitizen });
                    table.ForeignKey(
                        name: "FK_GarbageBinCitizens_Citizens_IdCitizen",
                        column: x => x.IdCitizen,
                        principalTable: "Citizens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GarbageBinCitizens_GarbageBins_IdGarbageBin",
                        column: x => x.IdGarbageBin,
                        principalTable: "GarbageBins",
                        principalColumn: "IdGarbageBin",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarbageCollections_IdGarbageBin",
                table: "GarbageCollections",
                column: "IdGarbageBin");

            migrationBuilder.CreateIndex(
                name: "IX_GarbageBinCitizens_IdCitizen",
                table: "GarbageBinCitizens",
                column: "IdCitizen");

            migrationBuilder.AddForeignKey(
                name: "FK_GarbageCollections_GarbageBins_IdGarbageBin",
                table: "GarbageCollections",
                column: "IdGarbageBin",
                principalTable: "GarbageBins",
                principalColumn: "IdGarbageBin",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarbageCollections_GarbageBins_IdGarbageBin",
                table: "GarbageCollections");

            migrationBuilder.DropTable(
                name: "GarbageBinCitizens");

            migrationBuilder.DropTable(
                name: "Citizens");

            migrationBuilder.DropTable(
                name: "GarbageBins");

            migrationBuilder.DropIndex(
                name: "IX_GarbageCollections_IdGarbageBin",
                table: "GarbageCollections");
        }
    }
}
