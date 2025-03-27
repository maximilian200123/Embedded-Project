using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarbageCollectionApp.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeToGarbageCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "GarbageCollections",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "GarbageCollections",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "GarbageCollections",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "GarbageCollections");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "GarbageCollections");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "GarbageCollections");
        }
    }
}
