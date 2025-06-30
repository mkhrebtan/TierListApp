using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TierList.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ImageStoresGuidForStorageAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Image");

            migrationBuilder.AddColumn<Guid>(
                name: "StorageKey",
                table: "Image",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "Image");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Image",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
