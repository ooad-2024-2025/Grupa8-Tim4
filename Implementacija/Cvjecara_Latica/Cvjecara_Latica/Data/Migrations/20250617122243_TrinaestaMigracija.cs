using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cvjecara_Latica.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrinaestaMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPickedUp",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPickedUp",
                table: "Orders");
        }
    }
}
