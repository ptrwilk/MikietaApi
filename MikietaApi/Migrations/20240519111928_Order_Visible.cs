using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MikietaApi.Migrations
{
    /// <inheritdoc />
    public partial class Order_Visible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Orders");
        }
    }
}
