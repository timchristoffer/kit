using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kit_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnsForAnalysReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "AnalysisReport");

            migrationBuilder.DropColumn(
                name: "SnippetId",
                table: "AnalysisReport");

            migrationBuilder.AddColumn<List<string>>(
                name: "PerformanceIssues",
                table: "AnalysisReport",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "ReadabilityIssues",
                table: "AnalysisReport",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "SecurityIssues",
                table: "AnalysisReport",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformanceIssues",
                table: "AnalysisReport");

            migrationBuilder.DropColumn(
                name: "ReadabilityIssues",
                table: "AnalysisReport");

            migrationBuilder.DropColumn(
                name: "SecurityIssues",
                table: "AnalysisReport");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "AnalysisReport",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SnippetId",
                table: "AnalysisReport",
                type: "uuid",
                nullable: true);
        }
    }
}
