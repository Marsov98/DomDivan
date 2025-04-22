using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomDivan.Migrations
{
    /// <inheritdoc />
    public partial class DB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Armchairs_Products_Id",
                table: "Armchairs");

            migrationBuilder.DropForeignKey(
                name: "FK_Beds_Products_Id",
                table: "Beds");

            migrationBuilder.DropForeignKey(
                name: "FK_Sofas_Products_Id",
                table: "Sofas");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Sofas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Beds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Armchairs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sofas_ProductId",
                table: "Sofas",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Beds_ProductId",
                table: "Beds",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Armchairs_ProductId",
                table: "Armchairs",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Armchairs_Products_ProductId",
                table: "Armchairs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Beds_Products_ProductId",
                table: "Beds",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sofas_Products_ProductId",
                table: "Sofas",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Armchairs_Products_ProductId",
                table: "Armchairs");

            migrationBuilder.DropForeignKey(
                name: "FK_Beds_Products_ProductId",
                table: "Beds");

            migrationBuilder.DropForeignKey(
                name: "FK_Sofas_Products_ProductId",
                table: "Sofas");

            migrationBuilder.DropIndex(
                name: "IX_Sofas_ProductId",
                table: "Sofas");

            migrationBuilder.DropIndex(
                name: "IX_Beds_ProductId",
                table: "Beds");

            migrationBuilder.DropIndex(
                name: "IX_Armchairs_ProductId",
                table: "Armchairs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Sofas");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Beds");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Armchairs");

            migrationBuilder.AddForeignKey(
                name: "FK_Armchairs_Products_Id",
                table: "Armchairs",
                column: "Id",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Beds_Products_Id",
                table: "Beds",
                column: "Id",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sofas_Products_Id",
                table: "Sofas",
                column: "Id",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
