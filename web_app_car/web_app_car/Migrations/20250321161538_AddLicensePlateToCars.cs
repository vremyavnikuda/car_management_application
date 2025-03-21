using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_app_car.Migrations
{
    /// <inheritdoc />
    public partial class AddLicensePlateToCars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicensePlate",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Power",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RepairDates",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Transmission",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicensePlate",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "RepairDates",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Cars");
        }
    }
}
