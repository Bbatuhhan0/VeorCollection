using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeorCollection.Migrations
{
    /// <inheritdoc />
    public partial class DinamikSistemGecis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Genders_GenderId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ScentTypes_ScentTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "ScentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Products_GenderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ScentTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ScentTypeId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductAttributeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_ProductAttributes_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalTable: "ProductAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductProductAttributeValue",
                columns: table => new
                {
                    AttributeValuesId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProductAttributeValue", x => new { x.AttributeValuesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_ProductProductAttributeValue_ProductAttributeValues_AttributeValuesId",
                        column: x => x.AttributeValuesId,
                        principalTable: "ProductAttributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductProductAttributeValue_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductAttributeId",
                table: "ProductAttributeValues",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductAttributeValue_ProductsId",
                table: "ProductProductAttributeValue",
                column: "ProductsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductProductAttributeValue");

            migrationBuilder.DropTable(
                name: "ProductAttributeValues");

            migrationBuilder.DropTable(
                name: "ProductAttributes");

            migrationBuilder.AddColumn<int>(
                name: "GenderId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScentTypeId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_GenderId",
                table: "Products",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ScentTypeId",
                table: "Products",
                column: "ScentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Genders_GenderId",
                table: "Products",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ScentTypes_ScentTypeId",
                table: "Products",
                column: "ScentTypeId",
                principalTable: "ScentTypes",
                principalColumn: "Id");
        }
    }
}
