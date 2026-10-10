using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth22.Migrations
{
    /// <inheritdoc />
    public partial class solvedCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolvedPuzzle",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SolvedPuzzle",
                table: "User");
        }
    }
}
