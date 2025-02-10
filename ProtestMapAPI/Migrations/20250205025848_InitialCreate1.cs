using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtestMapAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Protests",
                newName: "ZipCode");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Protests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Protests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Protests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Protests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Protests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Protests");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Protests");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Protests");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Protests");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Protests");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Protests",
                newName: "Address");
        }
    }
}
