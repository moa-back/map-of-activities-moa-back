using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapOfActivitiesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddImageName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Events",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Events");
        }
    }
}
