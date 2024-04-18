using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MikietaApi.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedOrderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlatNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FlatNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HomeNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Orders");
        }
    }
}
