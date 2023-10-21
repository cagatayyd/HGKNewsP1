using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HGKNews.Migrations
{
    /// <inheritdoc />
    public partial class MigrationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "NewsItems",
                newName: "NewsDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "NewsItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "NewsItems");

            migrationBuilder.RenameColumn(
                name: "NewsDate",
                table: "NewsItems",
                newName: "Date");
        }
    }
}
