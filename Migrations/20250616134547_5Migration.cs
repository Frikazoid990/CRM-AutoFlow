using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_AutoFlow.Migrations
{
    /// <inheritdoc />
    public partial class _5Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfigurationDetailsJson",
                table: "Deals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedConfiguration",
                table: "Deals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigurationDetailsJson",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "SelectedConfiguration",
                table: "Deals");
        }
    }
}
