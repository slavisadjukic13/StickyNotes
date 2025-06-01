using Microsoft.EntityFrameworkCore.Migrations;

namespace StickyNotes.Migrations
{
    public partial class SeedAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsAdmin", "IsDisabled", "PasswordHash", "Username" },
                values: new object[] { 1, true, false, "AQAAAAEAACcQAAAAEBXVxgpEt7Of6Qe8wltsekLAOXKdnlL4SDIdEVHXBDlEiLCOZi8aN4yhi2TZUCv/1A==", "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
