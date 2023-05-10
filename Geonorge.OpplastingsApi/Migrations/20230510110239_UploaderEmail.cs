using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geonorge.OpplastingsApi.Migrations
{
    /// <inheritdoc />
    public partial class UploaderEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Datasets_DatasetId",
                table: "File");

            migrationBuilder.AlterColumn<int>(
                name: "DatasetId",
                table: "File",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploaderEmail",
                table: "File",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_File_Datasets_DatasetId",
                table: "File",
                column: "DatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Datasets_DatasetId",
                table: "File");

            migrationBuilder.DropColumn(
                name: "UploaderEmail",
                table: "File");

            migrationBuilder.AlterColumn<int>(
                name: "DatasetId",
                table: "File",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_File_Datasets_DatasetId",
                table: "File",
                column: "DatasetId",
                principalTable: "Datasets",
                principalColumn: "Id");
        }
    }
}
