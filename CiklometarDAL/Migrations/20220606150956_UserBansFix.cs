using Microsoft.EntityFrameworkCore.Migrations;

namespace CiklometarDAL.Migrations
{
    public partial class UserBansFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Bans",
                table: "Bans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bans",
                table: "Bans",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId",
                table: "Bans",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Bans",
                table: "Bans");

            migrationBuilder.DropIndex(
                name: "IX_Bans_UserId",
                table: "Bans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bans",
                table: "Bans",
                columns: new[] { "UserId", "OrganizationId" });
        }
    }
}
