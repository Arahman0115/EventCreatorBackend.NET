using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtestMapAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedlon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Protests");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Protests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Protests",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Protests",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
