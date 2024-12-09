using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voicemail.Migrations
{
    /// <inheritdoc />
    public partial class AddCallerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CallerName",
                table: "Calls",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallerName",
                table: "Calls");
        }
    }
}
