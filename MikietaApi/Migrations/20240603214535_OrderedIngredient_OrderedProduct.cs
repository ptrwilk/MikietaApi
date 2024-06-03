using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MikietaApi.Migrations
{
    /// <inheritdoc />
    public partial class OrderedIngredient_OrderedProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProducts");

            migrationBuilder.CreateTable(
                name: "OrderedIngredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IngredientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceSmall = table.Column<double>(type: "REAL", nullable: false),
                    PriceMedium = table.Column<double>(type: "REAL", nullable: false),
                    PriceLarge = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderedIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderedProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    ProductType = table.Column<int>(type: "INTEGER", nullable: false),
                    PizzaType = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Index = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderedProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "OrderOrderedProducts",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderedProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Ready = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderOrderedProducts", x => new { x.OrderId, x.OrderedProductId });
                    table.ForeignKey(
                        name: "FK_OrderOrderedProducts_OrderedProducts_OrderedProductId",
                        column: x => x.OrderedProductId,
                        principalTable: "OrderedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderOrderedProducts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderedIngredients_IngredientId",
                table: "OrderedIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedProductOrderedIngredient_OrderedProductsId",
                table: "OrderedProductOrderedIngredient",
                column: "OrderedProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedProducts_ProductId",
                table: "OrderedProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOrderedProducts_OrderedProductId",
                table: "OrderOrderedProducts",
                column: "OrderedProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderedProductOrderedIngredient");

            migrationBuilder.DropTable(
                name: "OrderOrderedProducts");

            migrationBuilder.DropTable(
                name: "OrderedIngredients");

            migrationBuilder.DropTable(
                name: "OrderedProducts");

            migrationBuilder.CreateTable(
                name: "OrderProducts",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Ready = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProducts", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_OrderProducts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductId",
                table: "OrderProducts",
                column: "ProductId");
        }
    }
}
