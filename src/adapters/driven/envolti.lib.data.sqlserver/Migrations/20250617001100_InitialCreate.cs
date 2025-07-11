﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace envolti.lib.data.sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>( type: "int", nullable: false )
                        .Annotation( "SqlServer:Identity", "1, 1" ),
                    OrderIdExternal = table.Column<int>( type: "int", nullable: false ),
                    TotalPrice = table.Column<decimal>( type: "decimal(18,2)", nullable: false ),
                    CreatedAt = table.Column<DateTime>( type: "datetime2", nullable: false ),
                    ProcessedIn = table.Column<DateTime>( type: "datetime2", nullable: true ),
                    Status = table.Column<int>( type: "int", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_Orders", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>( type: "int", nullable: false )
                        .Annotation( "SqlServer:Identity", "1, 1" ),
                    OrderId = table.Column<int>( type: "int", nullable: false ),
                    ProductIdExternal = table.Column<int>( type: "int", nullable: false ),
                    Name = table.Column<string>( type: "nvarchar(100)", maxLength: 100, nullable: false ),
                    Price = table.Column<decimal>( type: "decimal(18,2)", nullable: false ),
                    CreatedAt = table.Column<DateTime>( type: "datetime2", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_Products", x => x.Id );
                    table.ForeignKey(
                        name: "FK_Products_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrderId",
                table: "Products",
                column: "OrderId" );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable(
                name: "Products" );

            migrationBuilder.DropTable(
                name: "Orders" );
        }
    }
}
