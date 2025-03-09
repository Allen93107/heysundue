using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heysundue.Migrations.Article
{
    /// <inheritdoc />
    public partial class UpdateArticleMode672 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeetingID",
                table: "Sessionusers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessionusers_MeetingID",
                table: "Sessionusers",
                column: "MeetingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessionusers_Meetings_MeetingID",
                table: "Sessionusers",
                column: "MeetingID",
                principalTable: "Meetings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessionusers_Meetings_MeetingID",
                table: "Sessionusers");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Sessionusers_MeetingID",
                table: "Sessionusers");

            migrationBuilder.DropColumn(
                name: "MeetingID",
                table: "Sessionusers");
        }
    }
}
