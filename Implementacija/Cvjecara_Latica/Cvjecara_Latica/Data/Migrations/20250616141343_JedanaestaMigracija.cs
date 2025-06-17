using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cvjecara_Latica.Data.Migrations
{
    /// <inheritdoc />
    public partial class JedanaestaMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Reports");
        }
    }
}
