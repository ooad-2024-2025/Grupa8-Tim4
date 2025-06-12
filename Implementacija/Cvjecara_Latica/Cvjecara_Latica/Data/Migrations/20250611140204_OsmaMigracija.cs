using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cvjecara_Latica.Data.Migrations
{
    /// <inheritdoc />
    public partial class OsmaMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "Discounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PersonID",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "PersonID",
                table: "Discounts");
        }
    }
}
