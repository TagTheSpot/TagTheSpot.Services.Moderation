using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagTheSpot.Services.Moderation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CityName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotId",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Submissions",
                newName: "SpotType");

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "Submissions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityName",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "SpotType",
                table: "Submissions",
                newName: "Type");

            migrationBuilder.AddColumn<Guid>(
                name: "SpotId",
                table: "Submissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
