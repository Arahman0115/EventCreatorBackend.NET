using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtestMapAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrganizerIdFromProtest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Protests_AspNetUsers_OrganizerId",
                table: "Protests");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizerId",
                table: "Protests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Protests_AspNetUsers_OrganizerId",
                table: "Protests",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Protests_AspNetUsers_OrganizerId",
                table: "Protests");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizerId",
                table: "Protests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Protests_AspNetUsers_OrganizerId",
                table: "Protests",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
