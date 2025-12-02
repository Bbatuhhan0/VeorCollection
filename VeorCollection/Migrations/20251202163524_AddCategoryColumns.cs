using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeorCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cinsiyet",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "KokuTipi",
                table: "Products");
        }
    }
}
