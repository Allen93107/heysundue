using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heysundue.Migrations.Article
{
    /// <inheritdoc />
    public partial class UpdateMeetingSessionusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessionusers_Meetings_MeetingID",
                table: "Sessionusers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessionusers_Sessionusers_SessionuserID",
                table: "Sessionusers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessionusers",
                table: "Sessionusers");

            migrationBuilder.RenameTable(
                name: "Sessionusers",
                newName: "Sessionuser_Meeting5");

            migrationBuilder.RenameIndex(
                name: "IX_Sessionusers_SessionuserID",
                table: "Sessionuser_Meeting5",
                newName: "IX_Sessionuser_Meeting5_SessionuserID");

            migrationBuilder.RenameIndex(
                name: "IX_Sessionusers_MeetingID",
                table: "Sessionuser_Meeting5",
                newName: "IX_Sessionuser_Meeting5_MeetingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessionuser_Meeting5",
                table: "Sessionuser_Meeting5",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessionuser_Meeting5_Meetings_MeetingID",
                table: "Sessionuser_Meeting5",
                column: "MeetingID",
                principalTable: "Meetings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessionuser_Meeting5_Sessionuser_Meeting5_SessionuserID",
                table: "Sessionuser_Meeting5",
                column: "SessionuserID",
                principalTable: "Sessionuser_Meeting5",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessionuser_Meeting5_Meetings_MeetingID",
                table: "Sessionuser_Meeting5");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessionuser_Meeting5_Sessionuser_Meeting5_SessionuserID",
                table: "Sessionuser_Meeting5");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessionuser_Meeting5",
                table: "Sessionuser_Meeting5");

            migrationBuilder.RenameTable(
                name: "Sessionuser_Meeting5",
                newName: "Sessionusers");

            migrationBuilder.RenameIndex(
                name: "IX_Sessionuser_Meeting5_SessionuserID",
                table: "Sessionusers",
                newName: "IX_Sessionusers_SessionuserID");

            migrationBuilder.RenameIndex(
                name: "IX_Sessionuser_Meeting5_MeetingID",
                table: "Sessionusers",
                newName: "IX_Sessionusers_MeetingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessionusers",
                table: "Sessionusers",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessionusers_Meetings_MeetingID",
                table: "Sessionusers",
                column: "MeetingID",
                principalTable: "Meetings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessionusers_Sessionusers_SessionuserID",
                table: "Sessionusers",
                column: "SessionuserID",
                principalTable: "Sessionusers",
                principalColumn: "ID");
        }
    }
}
