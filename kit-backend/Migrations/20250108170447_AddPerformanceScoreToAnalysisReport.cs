using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kit_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceScoreToAnalysisReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerformanceScore",
                table: "AnalysisReport",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReadabilityScore",
                table: "AnalysisReport",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecurityScore",
                table: "AnalysisReport",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformanceScore",
                table: "AnalysisReport");

            migrationBuilder.DropColumn(
                name: "ReadabilityScore",
                table: "AnalysisReport");

            migrationBuilder.DropColumn(
                name: "SecurityScore",
                table: "AnalysisReport");
        }
    }
}
