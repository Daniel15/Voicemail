using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voicemail.Migrations
{
    /// <inheritdoc />
    public partial class AddTranscriptSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranscriptSummary",
                table: "Calls",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranscriptSummary",
                table: "Calls");
        }
    }
}
