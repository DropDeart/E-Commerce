using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LJussAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "ShippingAddresses");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "ShippingAddresses");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "ShippingAddresses",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "ShippingAddresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "ShippingAddresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ShippingAddresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_CityId",
                table: "ShippingAddresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_DistrictId",
                table: "ShippingAddresses",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingAddresses_Cities_CityId",
                table: "ShippingAddresses",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingAddresses_Districts_DistrictId",
                table: "ShippingAddresses",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Cities_CityId",
                table: "ShippingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Districts_DistrictId",
                table: "ShippingAddresses");

            migrationBuilder.DropIndex(
                name: "IX_ShippingAddresses_CityId",
                table: "ShippingAddresses");

            migrationBuilder.DropIndex(
                name: "IX_ShippingAddresses_DistrictId",
                table: "ShippingAddresses");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "ShippingAddresses");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "ShippingAddresses");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ShippingAddresses");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ShippingAddresses",
                newName: "PostalCode");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ShippingAddresses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ShippingAddresses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
