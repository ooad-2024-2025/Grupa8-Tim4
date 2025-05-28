using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cvjecara_Latica.Data.Migrations
{
    /// <inheritdoc />
    public partial class CetvrtaMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_EmployeeID",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "Reports",
                newName: "PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_EmployeeID",
                table: "Reports",
                newName: "IX_Reports_PersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_PersonID",
                table: "Reports",
                column: "PersonID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_PersonID",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "Reports",
                newName: "EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_PersonID",
                table: "Reports",
                newName: "IX_Reports_EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_EmployeeID",
                table: "Reports",
                column: "EmployeeID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
