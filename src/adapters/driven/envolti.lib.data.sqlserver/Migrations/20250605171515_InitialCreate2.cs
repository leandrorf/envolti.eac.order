using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace envolti.lib.data.sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime( 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified ) );

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime( 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified ) );

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedIn",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime( 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified ) );

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0 );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products" );

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Orders" );

            migrationBuilder.DropColumn(
                name: "ProcessedIn",
                table: "Orders" );

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders" );
        }
    }
}
