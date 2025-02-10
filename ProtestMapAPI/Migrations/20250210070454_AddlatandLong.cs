using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtestMapAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddlatandLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Protests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Protests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Protests");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Protests");
        }
    }
}
