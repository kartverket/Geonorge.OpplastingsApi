using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Geonorge.OpplastingsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowedFileFormats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileFormats",
                columns: table => new
                {
                    Extension = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileFormats", x => x.Extension);
                });

            migrationBuilder.CreateTable(
                name: "DatasetAllowedFileFormats",
                columns: table => new
                {
                    AllowedFileFormatsExtension = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    DatasetsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetAllowedFileFormats", x => new { x.AllowedFileFormatsExtension, x.DatasetsId });
                    table.ForeignKey(
                        name: "FK_DatasetAllowedFileFormats_Datasets_DatasetsId",
                        column: x => x.DatasetsId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatasetAllowedFileFormats_FileFormats_AllowedFileFormatsExtension",
                        column: x => x.AllowedFileFormatsExtension,
                        principalTable: "FileFormats",
                        principalColumn: "Extension",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FileFormats",
                columns: new[] { "Extension", "Name" },
                values: new object[,]
                {
                    { "gdb", "ESRI file Geodatabase" },
                    { "gml", "Geography Markup Language" },
                    { "gpkg", "GeoPackage" },
                    { "shp", "Shape" },
                    { "sos", "Samordnet Opplegg for Stedfestet Informasjon (SOSI)" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatasetAllowedFileFormats_DatasetsId",
                table: "DatasetAllowedFileFormats",
                column: "DatasetsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatasetAllowedFileFormats");

            migrationBuilder.DropTable(
                name: "FileFormats");
        }
    }
}
