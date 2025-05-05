using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomDivan.Migrations
{
    /// <inheritdoc />
    public partial class UpDB4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsInSupply_Supplyes_SupplyId",
                table: "ProductsInSupply");

            migrationBuilder.DropForeignKey(
                name: "FK_Supplyes_Suppliers_SupplierId",
                table: "Supplyes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Supplyes",
                table: "Supplyes");

            migrationBuilder.RenameTable(
                name: "Supplyes",
                newName: "Supplies");

            migrationBuilder.RenameIndex(
                name: "IX_Supplyes_SupplierId",
                table: "Supplies",
                newName: "IX_Supplies_SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Supplies",
                table: "Supplies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsInSupply_Supplies_SupplyId",
                table: "ProductsInSupply",
                column: "SupplyId",
                principalTable: "Supplies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Supplies_Suppliers_SupplierId",
                table: "Supplies",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsInSupply_Supplies_SupplyId",
                table: "ProductsInSupply");

            migrationBuilder.DropForeignKey(
                name: "FK_Supplies_Suppliers_SupplierId",
                table: "Supplies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Supplies",
                table: "Supplies");

            migrationBuilder.RenameTable(
                name: "Supplies",
                newName: "Supplyes");

            migrationBuilder.RenameIndex(
                name: "IX_Supplies_SupplierId",
                table: "Supplyes",
                newName: "IX_Supplyes_SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Supplyes",
                table: "Supplyes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsInSupply_Supplyes_SupplyId",
                table: "ProductsInSupply",
                column: "SupplyId",
                principalTable: "Supplyes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Supplyes_Suppliers_SupplierId",
                table: "Supplyes",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
