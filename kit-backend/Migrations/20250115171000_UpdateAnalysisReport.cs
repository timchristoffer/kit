using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kit_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnalysisReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "AnalysisReport",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "AnalysisReport");
        }
    }
}
