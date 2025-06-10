using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_AutoFlow.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Complectation",
                table: "Cars");

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationsJson",
                table: "Cars",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigurationsJson",
                table: "Cars");

            migrationBuilder.AddColumn<string>(
                name: "Complectation",
                table: "Cars",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
