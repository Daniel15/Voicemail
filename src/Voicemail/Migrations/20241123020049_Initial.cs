using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voicemail.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: true),
                    NumberFrom = table.Column<string>(type: "TEXT", nullable: true),
                    NumberForwardedFrom = table.Column<string>(type: "TEXT", nullable: true),
                    NumberTo = table.Column<string>(type: "TEXT", nullable: true),
                    RecordingDurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    RecordingUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calls", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calls_ExternalId",
                table: "Calls",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calls");
        }
    }
}
