using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MikietaApi.Migrations
{
    /// <inheritdoc />
    public partial class OrderedProduct_OrderedIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderedProductOrderedIngredient");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderedIngredientEntityId",
                table: "OrderedProducts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderedProductOrderedIngredients",
                columns: table => new
                {
                    OrderedProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderedIngredientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAdditionalIngredient = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedProductOrderedIngredients", x => new { x.OrderedProductId, x.OrderedIngredientId });
                    table.ForeignKey(
                        name: "FK_OrderedProductOrderedIngredients_OrderedIngredients_OrderedIngredientId",
                        column: x => x.OrderedIngredientId,
                        principalTable: "OrderedIngredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderedProductOrderedIngredients_OrderedProducts_OrderedProductId",
                        column: x => x.OrderedProductId,
                        principalTable: "OrderedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderedProducts_OrderedIngredientEntityId",
                table: "OrderedProducts",
                column: "OrderedIngredientEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedProductOrderedIngredients_OrderedIngredientId",
                table: "OrderedProductOrderedIngredients",
                column: "OrderedIngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedProducts_OrderedIngredients_OrderedIngredientEntityId",
                table: "OrderedProducts",
                column: "OrderedIngredientEntityId",
                principalTable: "OrderedIngredients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedProducts_OrderedIngredients_OrderedIngredientEntityId",
                table: "OrderedProducts");

            migrationBuilder.DropTable(
                name: "OrderedProductOrderedIngredients");

            migrationBuilder.DropIndex(
                name: "IX_OrderedProducts_OrderedIngredientEntityId",
                table: "OrderedProducts");

            migrationBuilder.DropColumn(
                name: "OrderedIngredientEntityId",
                table: "OrderedProducts");

            migrationBuilder.CreateTable(
                name: "OrderedProductOrderedIngredient",
                columns: table => new
                {
                    OrderedIngredientsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderedProductsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedProductOrderedIngredient", x => new { x.OrderedIngredientsId, x.OrderedProductsId });
                    table.ForeignKey(
                        name: "FK_OrderedProductOrderedIngredient_OrderedIngredients_OrderedIngredientsId",
                        column: x => x.OrderedIngredientsId,
                        principalTable: "OrderedIngredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderedProductOrderedIngredient_OrderedProducts_OrderedProductsId",
                        column: x => x.OrderedProductsId,
                        principalTable: "OrderedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderedProductOrderedIngredient_OrderedProductsId",
                table: "OrderedProductOrderedIngredient",
                column: "OrderedProductsId");
        }
    }
}
