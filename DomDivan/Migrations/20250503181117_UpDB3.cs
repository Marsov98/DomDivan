using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomDivan.Migrations
{
    /// <inheritdoc />
    public partial class UpDB3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Supplyes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplyDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplyes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supplyes_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsInSupply",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplyId = table.Column<int>(type: "INTEGER", nullable: false),
                    VariantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsInSupply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsInSupply_Supplyes_SupplyId",
                        column: x => x.SupplyId,
                        principalTable: "Supplyes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsInSupply_Variants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "Variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsInSupply_SupplyId",
                table: "ProductsInSupply",
                column: "SupplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsInSupply_VariantId",
                table: "ProductsInSupply",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplyes_SupplierId",
                table: "Supplyes",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsInSupply");

            migrationBuilder.DropTable(
                name: "Supplyes");
        }
    }
}
