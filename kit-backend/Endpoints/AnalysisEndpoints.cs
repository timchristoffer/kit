using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using KitBackend.Models.Requests;
using KitBackend.Models.Responses;
using KitBackend.Services;
using System.Text;
using System.IO;

namespace KitBackend.Endpoints
{
    public static class AnalysisEndpoints
    {
        public static void MapAnalysisEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/analysis", AnalyzeCode)
               .WithName("AnalyzeCode")
               .Produces<AnalysisReport>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status500InternalServerError)
               .WithTags("Analysis");

            app.MapGet("/api/analysis/{id:guid}", GetAnalysisReport)
               .WithName("GetAnalysisReport")
               .Produces<AnalysisReport>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .WithTags("Analysis");

            app.MapGet("/api/analysis/download/{id:guid}", DownloadReport)
               .WithName("DownloadReport")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .WithTags("Analysis");
        }

        public static async Task<IResult> AnalyzeCode(AnalysisRequest request, IAnalysisService analysisService, IFileService fileService)
        {
            try
            {
                string codeToAnalyze = string.Empty;

                if (request.SourceType == "file" && request.FileId.HasValue)
                {
                    var file = await fileService.GetFileById(request.FileId.Value);
                    if (file == null)
                    {
                        return Results.NotFound("File not found.");
                    }
                    codeToAnalyze = Encoding.UTF8.GetString(file.FileContent);
                }
                else if (!string.IsNullOrEmpty(request.Content))
                {
                    codeToAnalyze = request.Content;
                }

                if (string.IsNullOrEmpty(codeToAnalyze))
                {
                    return Results.BadRequest("No code provided.");
                }

                var analysisReport = await analysisService.GenerateReportAsync(codeToAnalyze);

                return Results.Ok(analysisReport);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: $"Code analysis failed: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> GetAnalysisReport(Guid id, IAnalysisService analysisService)
        {
            try
            {
                var report = await analysisService.GetReportByIdAsync(id);
                return report != null ? Results.Ok(report) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: $"Failed to retrieve analysis report: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        public static async Task<IResult> DownloadReport(Guid id, IAnalysisService analysisService)
        {
            try
            {
                var report = await analysisService.GetReportByIdAsync(id);
                if (report == null)
                {
                    return Results.NotFound("Report not found.");
                }

                // Kontrollera om PDF-innehållet finns i databasen
                if (report.PdfContent != null && report.PdfContent.Length > 0)
                {
                    return Results.File(report.PdfContent, "application/pdf", $"{id}.pdf");
                }
                
                // Fallback: om innehållet inte finns i databasen, försök läsa från filen
                if (!string.IsNullOrEmpty(report.PdfPath) && File.Exists(report.PdfPath))
                {
                    var pdfBytes = await File.ReadAllBytesAsync(report.PdfPath);
                    return Results.File(pdfBytes, "application/pdf", $"{id}.pdf");
                }

                return Results.NotFound("PDF not found for this report.");
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: $"Failed to download report: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}