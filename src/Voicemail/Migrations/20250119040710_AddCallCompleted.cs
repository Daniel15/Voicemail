using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voicemail.Migrations
{
    /// <inheritdoc />
    public partial class AddCallCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "completed",
                table: "Calls",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "completed",
                table: "Calls");
        }
    }
}
