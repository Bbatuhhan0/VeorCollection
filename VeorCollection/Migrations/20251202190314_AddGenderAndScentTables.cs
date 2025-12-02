using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeorCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderAndScentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cinsiyet",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "KokuTipi",
                table: "Products");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Cinsiyet",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KokuTipi",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
