using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geonorge.OpplastingsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddContactEmailExtra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmailExtra",
                table: "Datasets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmailExtra",
                table: "Datasets");
        }
    }
}
