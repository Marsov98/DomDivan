using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomDivan.Migrations
{
    /// <inheritdoc />
    public partial class DelDB5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastBuyPrice",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Photos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LastBuyPrice",
                table: "Variants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Photos",
                type: "TEXT",
                nullable: true);
        }
    }
}
