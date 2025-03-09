using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heysundue.Migrations.Article
{
    /// <inheritdoc />
    public partial class Updat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Joinlists_Joinlists_JoinlistID",
                table: "Joinlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessionusers_Sessionusers_SessionuserID",
                table: "Sessionusers");

            migrationBuilder.DropIndex(
                name: "IX_Sessionusers_SessionuserID",
                table: "Sessionusers");

            migrationBuilder.DropIndex(
                name: "IX_Joinlists_JoinlistID",
                table: "Joinlists");

            migrationBuilder.DropColumn(
                name: "SessionuserID",
                table: "Sessionusers");

            migrationBuilder.DropColumn(
                name: "JoinlistID",
                table: "Joinlists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionuserID",
                table: "Sessionusers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JoinlistID",
                table: "Joinlists",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessionusers_SessionuserID",
                table: "Sessionusers",
                column: "SessionuserID");

            migrationBuilder.CreateIndex(
                name: "IX_Joinlists_JoinlistID",
                table: "Joinlists",
                column: "JoinlistID");

            migrationBuilder.AddForeignKey(
                name: "FK_Joinlists_Joinlists_JoinlistID",
                table: "Joinlists",
                column: "JoinlistID",
                principalTable: "Joinlists",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessionusers_Sessionusers_SessionuserID",
                table: "Sessionusers",
                column: "SessionuserID",
                principalTable: "Sessionusers",
                principalColumn: "ID");
        }
    }
}
