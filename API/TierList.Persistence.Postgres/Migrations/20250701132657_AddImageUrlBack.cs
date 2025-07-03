using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TierList.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Image",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Image");
        }
    }
}
