using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geonorge.OpplastingsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUploaderUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploaderUsername",
                table: "File",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploaderUsername",
                table: "File");
        }
    }
}
