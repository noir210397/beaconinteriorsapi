using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace beaconinteriorsapi.Migrations
{
    /// <inheritdoc />
    public partial class AddedTypeToAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                table: "Address",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "Address");
        }
    }
}
