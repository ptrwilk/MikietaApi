using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MikietaApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSmallPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceSmall",
                table: "OrderedIngredients");

            migrationBuilder.DropColumn(
                name: "PriceSmall",
                table: "Ingredients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceSmall",
                table: "OrderedIngredients",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceSmall",
                table: "Ingredients",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
