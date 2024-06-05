using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MikietaApi.Migrations
{
    /// <inheritdoc />
    public partial class OrderedProductOrderedIngredient_IsIngredientRemoved_ReplacedIngredientId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIngredientRemoved",
                table: "OrderedProductOrderedIngredients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ReplacedIngredientId",
                table: "OrderedProductOrderedIngredients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderedProductOrderedIngredients_ReplacedIngredientId",
                table: "OrderedProductOrderedIngredients",
                column: "ReplacedIngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedProductOrderedIngredients_OrderedIngredients_ReplacedIngredientId",
                table: "OrderedProductOrderedIngredients",
                column: "ReplacedIngredientId",
                principalTable: "OrderedIngredients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedProductOrderedIngredients_OrderedIngredients_ReplacedIngredientId",
                table: "OrderedProductOrderedIngredients");

            migrationBuilder.DropIndex(
                name: "IX_OrderedProductOrderedIngredients_ReplacedIngredientId",
                table: "OrderedProductOrderedIngredients");

            migrationBuilder.DropColumn(
                name: "IsIngredientRemoved",
                table: "OrderedProductOrderedIngredients");

            migrationBuilder.DropColumn(
                name: "ReplacedIngredientId",
                table: "OrderedProductOrderedIngredients");
        }
    }
}
