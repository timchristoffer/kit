using KitBackend.Models.Responses;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace KitBackend.Services
{
    public class PdfReportService
    {
        private readonly ILogger<PdfReportService> _logger;

        // Statisk konstruktor för att konfigurera PDF-miljö
        static PdfReportService()
        {
            // Sätt kulturinställningar för att undvika problem med formatering
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            
            // Registrera teckenkodshantering
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public PdfReportService(ILogger<PdfReportService> logger)
        {
            _logger = logger;
        }

        public (string filePath, byte[] content) GeneratePdfReport(AnalysisReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report cannot be null.");
            }

            _logger.LogInformation("Starting PDF generation for report ID: {ReportId}", report.ReportId);

            // Säkerställ att alla listor är initialiserade
            report.Issues ??= new List<string>();
            report.SecurityIssues ??= new List<string>();
            report.PerformanceIssues ??= new List<string>();
            report.ReadabilityIssues ??= new List<string>();
            report.BestPracticesFeedback ??= string.Empty;

            var tempPath = Path.GetTempPath();
            var reportFolder = Path.Combine(tempPath, "Reports");
            
            try
            {
                Directory.CreateDirectory(reportFolder);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not create directory {Path}, using temp path instead", reportFolder);
                reportFolder = tempPath;
            }
            
            var pdfPath = Path.Combine(reportFolder, $"{report.ReportId}.pdf");
            byte[] pdfContent;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Skapa en PDF med iText 7
                    using (var writer = new PdfWriter(memoryStream))
                    {
                        using (var pdf = new PdfDocument(writer))
                        {
                            var document = new Document(pdf);

                            // Lägg till titel
                            document.Add(new Paragraph("Analysis Report")
                                .SetFontSize(20)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));

                            document.Add(new Paragraph($"Report ID: {report.ReportId}"));
                            document.Add(new Paragraph("\n"));

                            // Lägg till poäng
                            document.Add(new Paragraph("Scores")
                                .SetFontSize(16)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
                            document.Add(new Paragraph($"Complexity: {report.ComplexityScore}"));
                            document.Add(new Paragraph($"Readability: {report.ReadabilityScore}"));
                            document.Add(new Paragraph($"Security: {report.SecurityScore}"));
                            document.Add(new Paragraph($"Performance: {report.PerformanceScore}"));
                            document.Add(new Paragraph("\n"));

                            // Lägg till problem
                            document.Add(new Paragraph("Issues")
                                .SetFontSize(16)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
                            foreach (var issue in report.Issues)
                            {
                                document.Add(new Paragraph($"• {issue}"));
                            }
                            document.Add(new Paragraph("\n"));

                            document.Add(new Paragraph("Security Issues")
                                .SetFontSize(16)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
                            foreach (var issue in report.SecurityIssues)
                            {
                                document.Add(new Paragraph($"• {issue}"));
                            }
                            document.Add(new Paragraph("\n"));

                            document.Add(new Paragraph("Performance Issues")
                                .SetFontSize(16)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
                            foreach (var issue in report.PerformanceIssues)
                            {
                                document.Add(new Paragraph($"• {issue}"));
                            }
                            document.Add(new Paragraph("\n"));

                            document.Add(new Paragraph("Readability Issues")
                                .SetFontSize(16)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
                            foreach (var issue in report.ReadabilityIssues)
                            {
                                document.Add(new Paragraph($"• {issue}"));
                            }
                            document.Add(new Paragraph("\n"));

                            document.Add(new Paragraph("Best Practices Feedback")
                                .SetFontSize(16)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
                            document.Add(new Paragraph(report.BestPracticesFeedback));
                        }
                    }

                    pdfContent = memoryStream.ToArray();
                }

                // Försök spara till fil om möjligt
                try
                {
                    File.WriteAllBytes(pdfPath, pdfContent);
                    _logger.LogInformation("PDF saved to {Path}", pdfPath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not save PDF to file system, but PDF was generated in memory");
                    pdfPath = "Memory only - " + report.ReportId.ToString();
                }

                _logger.LogInformation("PDF generation completed successfully for report ID: {ReportId}", report.ReportId);
                return (pdfPath, pdfContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF generation failed: {ErrorMessage}", ex.Message);
                throw new Exception($"Failed to generate PDF report: {ex.Message}", ex);
            }
        }
    }
}