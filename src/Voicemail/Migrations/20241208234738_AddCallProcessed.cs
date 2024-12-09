using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voicemail.Migrations
{
    /// <inheritdoc />
    public partial class AddCallProcessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "Calls",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "Calls");
        }
    }
}
