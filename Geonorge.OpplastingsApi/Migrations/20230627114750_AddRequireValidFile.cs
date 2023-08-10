using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geonorge.OpplastingsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRequireValidFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireValidFile",
                table: "Datasets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireValidFile",
                table: "Datasets");
        }
    }
}
