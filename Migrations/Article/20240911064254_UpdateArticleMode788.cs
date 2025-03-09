using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heysundue.Migrations.Article
{
    /// <inheritdoc />
    public partial class UpdateArticleMode788 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Joinlists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Joinlists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Joinlists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDNumber",
                table: "Joinlists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityType1",
                table: "Joinlists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityType2",
                table: "Joinlists",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Joinlists");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Joinlists");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Joinlists");

            migrationBuilder.DropColumn(
                name: "IDNumber",
                table: "Joinlists");

            migrationBuilder.DropColumn(
                name: "IdentityType1",
                table: "Joinlists");

            migrationBuilder.DropColumn(
                name: "IdentityType2",
                table: "Joinlists");
        }
    }
}
