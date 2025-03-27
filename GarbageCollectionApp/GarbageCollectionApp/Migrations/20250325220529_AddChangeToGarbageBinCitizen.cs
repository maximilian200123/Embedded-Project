using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarbageCollectionApp.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeToGarbageBinCitizen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add unique constraint on IdGarbageBin column in GarbageBinCitizens table
            migrationBuilder.AddUniqueConstraint(
                name: "AK_GarbageBinCitizens_IdGarbageBin",
                table: "GarbageBinCitizens",
                column: "IdGarbageBin");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the unique constraint if we want to revert the migration
            migrationBuilder.DropUniqueConstraint(
                name: "AK_GarbageBinCitizens_IdGarbageBin",
                table: "GarbageBinCitizens");
        }
    }
}
