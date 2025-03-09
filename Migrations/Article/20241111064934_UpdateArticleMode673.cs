using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heysundue.Migrations.Article
{
    /// <inheritdoc />
    public partial class UpdateArticleMode673 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessionusers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: true),
                    RegNo = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    ChineseName = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    RegistrationStatus = table.Column<string>(type: "TEXT", nullable: true),
                    IdentityType1 = table.Column<string>(type: "TEXT", nullable: true),
                    IdentityType2 = table.Column<string>(type: "TEXT", nullable: true),
                    IDNumber = table.Column<string>(type: "TEXT", nullable: true),
                    SessionuserID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessionusers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sessionusers_Sessionusers_SessionuserID",
                        column: x => x.SessionuserID,
                        principalTable: "Sessionusers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessionusers_SessionuserID",
                table: "Sessionusers",
                column: "SessionuserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessionusers");
        }
    }
}
