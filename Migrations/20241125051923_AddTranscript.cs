using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voicemail.Migrations
{
    /// <inheritdoc />
    public partial class AddTranscript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MentionedCompanies",
                table: "Calls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MentionedNumbers",
                table: "Calls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MentionedPeople",
                table: "Calls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TranscriptText",
                table: "Calls",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MentionedCompanies",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "MentionedNumbers",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "MentionedPeople",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "TranscriptText",
                table: "Calls");
        }
    }
}
