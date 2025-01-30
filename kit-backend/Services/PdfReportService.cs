using KitBackend.Models.Responses;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Charting;
using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace KitBackend.Services
{
    public class PdfReportService
    {
        private readonly ILogger<PdfReportService> _logger;

        public PdfReportService(ILogger<PdfReportService> logger)
        {
            _logger = logger;
        }

        public string GeneratePdfReport(AnalysisReport report)
        {
            var pdfPath = Path.Combine("Reports", $"{report.ReportId}.pdf");
            Directory.CreateDirectory("Reports");

            try
            {
                using (var document = new PdfDocument())
                {
                    var page = document.AddPage();
                    var gfx = XGraphics.FromPdfPage(page);
                    var title = new XFont("Verdana", 20, XFontStyleEx.Bold);
                    var font = new XFont("Verdana", 12);
                    var boldFont = new XFont("Verdana", 12, XFontStyleEx.Bold);

                    double margin = 40;
                    double yOffset = margin;

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

                    void DrawWrappedString(string text, XFont font, XBrush brush, double x, double y)
                    {
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

                    DrawWrappedString("Analysis Report", title, XBrushes.Black, margin, yOffset);
                    DrawWrappedString($"Report ID: {report.ReportId}", font, XBrushes.Black, margin, yOffset);

                    gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                    yOffset += 10;

                    DrawWrappedString("Scores", boldFont, XBrushes.Black, margin, yOffset);
                    yOffset += 10; // Add some space between "Scores" and the chart

                    // Create a vertical column chart
                    var chart = new Chart(ChartType.Column2D);
                    var series = chart.SeriesCollection.AddSeries();
                    series.Add(report.ComplexityScore);
                    series.Add(report.ReadabilityScore);
                    series.Add(report.SecurityScore);
                    series.Add(report.PerformanceScore);

                    // Add labels for the X-axis using XSeries
                    var labels = new[] { "Complexity", "Readability", "Security", "Performance" };
                    var xSeries = new XSeries();
                    foreach (var label in labels)
                    {
                        xSeries.Add(label);
                    }
                    chart.XValues.Add(xSeries);

                    // Configure the X-axis
                    var xAxis = chart.XAxis;
                    xAxis.HasMajorGridlines = true;
                    xAxis.MajorGridlines.LineFormat.Color = XColors.LightGray;
                    xAxis.MajorTickMark = TickMarkType.Outside;
                    xAxis.MinorTickMark = TickMarkType.None;

                    // Configure the Y-axis
                    var yAxis = chart.YAxis;
                    yAxis.Title.Caption = "Values";
                    yAxis.HasMajorGridlines = true;
                    yAxis.MajorGridlines.LineFormat.Color = XColors.LightGray;
                    yAxis.MinimumScale = 0;
                    yAxis.MaximumScale = 100;
                    yAxis.MajorTickMark = TickMarkType.Outside;
                    yAxis.MinorTickMark = TickMarkType.None;
                    yAxis.MajorTick = 20;

                    // Customize the series with specific RGBA colors
                    series.Elements[0].FillFormat.Color = XColor.FromArgb(51, 255, 99, 132); // Complexity (rgba(255, 99, 132, 0.2))
                    series.Elements[1].FillFormat.Color = XColor.FromArgb(51, 54, 162, 235); // Readability (rgba(54, 162, 235, 0.2))
                    series.Elements[2].FillFormat.Color = XColor.FromArgb(51, 255, 206, 86); // Security (rgba(255, 206, 86, 0.2))
                    series.Elements[3].FillFormat.Color = XColor.FromArgb(51, 75, 192, 192); // Performance (rgba(75, 192, 192, 0.2))

                    // Add chart to frame and render
                    var chartFrame = new ChartFrame
                    {
                        Location = new XPoint(margin, yOffset),
                        Size = new XSize(page.Width - 2 * margin, 300),
                    };
                    chartFrame.Add(chart);
                    chartFrame.Draw(gfx);

                    yOffset += 310;

                    gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                    yOffset += 10;

                    DrawWrappedString("Issues", boldFont, XBrushes.Black, margin, yOffset);
                    foreach (var issue in report.Issues)
                    {
                        DrawWrappedString(issue, font, XBrushes.Black, margin, yOffset);
                    }

                    gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                    yOffset += 10;

                    DrawWrappedString("Security Issues", boldFont, XBrushes.Black, margin, yOffset);
                    foreach (var issue in report.SecurityIssues)
                    {
                        DrawWrappedString(issue, font, XBrushes.Black, margin, yOffset);
                    }

                    gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                    yOffset += 10;

                    DrawWrappedString("Performance Issues", boldFont, XBrushes.Black, margin, yOffset);
                    foreach (var issue in report.PerformanceIssues)
                    {
                        DrawWrappedString(issue, font, XBrushes.Black, margin, yOffset);
                    }

                    gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                    yOffset += 10;

                    DrawWrappedString("Readability Issues", boldFont, XBrushes.Black, margin, yOffset);
                    foreach (var issue in report.ReadabilityIssues)
                    {
                        DrawWrappedString(issue, font, XBrushes.Black, margin, yOffset);
                    }

                    gfx.DrawLine(XPens.Black, margin, yOffset, page.Width - margin, yOffset);
                    yOffset += 10;

                    DrawWrappedString($"Best Practices Feedback: {report.BestPracticesFeedback}", boldFont, XBrushes.Black, margin, yOffset);
                    //DrawWrappedString($"Explanation: {report.Explanation}", boldFont, XBrushes.Black, margin, yOffset);

                    document.Save(pdfPath);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "PDF generation failed with IOException");
                throw new Exception("Failed to generate PDF report: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF generation failed with an unknown exception");
                throw new Exception("Failed to generate PDF report: " + ex.Message);
            }

            return pdfPath;
        }
    }
}