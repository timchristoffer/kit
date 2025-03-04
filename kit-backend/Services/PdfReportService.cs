using KitBackend.Models.Responses;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Charting;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading;
using System.Globalization;

namespace KitBackend.Services
{
    public class PdfReportService
    {
        private readonly ILogger<PdfReportService> _logger;

        // Statisk konstruktor för att konfigurera PDF-miljön
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

        // Ändra returtypen till en tuple med både sökväg och innehåll
        public (string filePath, byte[] content) GeneratePdfReport(AnalysisReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report cannot be null.");
            }

            _logger.LogInformation("Starting PDF generation for report ID: {ReportId}", report.ReportId);

            // Kontrollera att alla listor är initialiserade om de finns
            report.Issues ??= new List<string>();
            report.SecurityIssues ??= new List<string>();
            report.PerformanceIssues ??= new List<string>();
            report.ReadabilityIssues ??= new List<string>();
            report.BestPracticesFeedback ??= string.Empty;

            var tempPath = Path.GetTempPath();
            var reportFolder = Path.Combine(tempPath, "Reports");
            Directory.CreateDirectory(reportFolder);
            var pdfPath = Path.Combine(reportFolder, $"{report.ReportId}.pdf");
            byte[] pdfContent;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var document = new PdfDocument())
                    {
                        var page = document.AddPage();
                        var gfx = XGraphics.FromPdfPage(page);
                        var title = new XFont("sans-serif", 20, XFontStyleEx.Bold);
                        var font = new XFont("sans-serif", 12);
                        var boldFont = new XFont("sans-serif", 12, XFontStyleEx.Bold);

                        double margin = 40;
                        double yOffset = margin;

                        // Funktion för att rita text
                        void DrawString(string text, XFont font, XBrush brush, double x, double y)
                        {
                            var size = gfx.MeasureString(text, font);
                            var rect = new XRect(x, y, page.Width - 2 * margin, page.Height - 2 * margin);
                            var format = XStringFormats.TopLeft;

                            if (y + size.Height > page.Height - margin)
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                yOffset = margin;
                                rect = new XRect(x, yOffset, page.Width - 2 * margin, page.Height - 2 * margin);
                            }

                            gfx.DrawString(text, font, brush, rect, format);
                            yOffset += size.Height;
                        }

                        // Funktion för att hantera text som ska brytas på flera rader
                        void DrawWrappedString(string text, XFont font, XBrush brush, double x, double y)
                        {
                            if (string.IsNullOrEmpty(text))
                                return;

                            var words = text.Split(' ');
                            var line = string.Empty;

                            foreach (var word in words)
                            {
                                var testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
                                var size = gfx.MeasureString(testLine, font);

                                if (size.Width > page.Width - 2 * margin)
                                {
                                    DrawString(line, font, brush, x, yOffset);
                                    line = word;
                                }
                                else
                                {
                                    line = testLine;
                                }
                            }

                            if (!string.IsNullOrEmpty(line))
                            {
                                DrawString(line, font, brush, x, yOffset);
                            }
                        }

                        // Lägg till rapportens titel och ID
                        DrawWrappedString("Analysis Report", title, XBrushes.Black, margin, yOffset);
                        DrawWrappedString($"Report ID: {report.ReportId}", font, XBrushes.Black, margin, yOffset);

                        gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                        yOffset += 10;

                        // Lägg till poäng utan diagram för bättre stabilitet
                        DrawWrappedString("Scores", boldFont, XBrushes.Black, margin, yOffset);
                        yOffset += 10;

                        DrawWrappedString($"Complexity: {report.ComplexityScore}", font, XBrushes.Black, margin, yOffset);
                        DrawWrappedString($"Readability: {report.ReadabilityScore}", font, XBrushes.Black, margin, yOffset);
                        DrawWrappedString($"Security: {report.SecurityScore}", font, XBrushes.Black, margin, yOffset);
                        DrawWrappedString($"Performance: {report.PerformanceScore}", font, XBrushes.Black, margin, yOffset);
                        
                        yOffset += 20;
                        gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                        yOffset += 10;

                        // Lägg till olika issue-sektioner
                        DrawWrappedString("Issues", boldFont, XBrushes.Black, margin, yOffset);
                        foreach (var issue in report.Issues)
                        {
                            DrawWrappedString($"• {issue}", font, XBrushes.Black, margin, yOffset);
                        }

                        gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                        yOffset += 10;

                        DrawWrappedString("Security Issues", boldFont, XBrushes.Black, margin, yOffset);
                        foreach (var issue in report.SecurityIssues)
                        {
                            DrawWrappedString($"• {issue}", font, XBrushes.Black, margin, yOffset);
                        }

                        gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                        yOffset += 10;

                        DrawWrappedString("Performance Issues", boldFont, XBrushes.Black, margin, yOffset);
                        foreach (var issue in report.PerformanceIssues)
                        {
                            DrawWrappedString($"• {issue}", font, XBrushes.Black, margin, yOffset);
                        }

                        gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                        yOffset += 10;

                        DrawWrappedString("Readability Issues", boldFont, XBrushes.Black, margin, yOffset);
                        foreach (var issue in report.ReadabilityIssues)
                        {
                            DrawWrappedString($"• {issue}", font, XBrushes.Black, margin, yOffset);
                        }

                        gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                        yOffset += 10;

                        DrawWrappedString($"Best Practices Feedback:", boldFont, XBrushes.Black, margin, yOffset);
                        DrawWrappedString(report.BestPracticesFeedback, font, XBrushes.Black, margin, yOffset);

                        // Spara dokumentet både till fil och till MemoryStream
                        document.Save(pdfPath);
                        document.Save(ms);
                    }
                    
                    // Hämta PDF-innehållet från MemoryStream
                    pdfContent = ms.ToArray();
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