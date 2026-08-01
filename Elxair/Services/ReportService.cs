using System;
using System.Composition;
using System.Linq;
using System.Text.Json;
using Elxair.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Elxair.Services
{
    public class ReportService
    {
        // ---------- Theme palette (matches the Elixir admin dashboard) ----------
        private const string ColorBackground = "#FAF6F0";   // cream page background
        private const string ColorPanel = "#FFFFFF";        // card / panel background
        private const string ColorPanelAlt = "#F7F1E6";     // alternating row background
        private const string ColorAccent = "#B08D57";       // gold accent (icons, bars, kickers)
        private const string ColorHeading = "#4A2E28";      // dark maroon-brown for headings/logo
        private const string ColorTextPrimary = "#2B2420";  // near-black brown body text
        private const string ColorTextSecondary = "#9C948A";// grey secondary / labels
        private const string ColorBorder = "#E8E0D2";       // light hairline border
        private const string FontName = "Times New Roman";



        // ---------- Shared page chrome ----------
        private void ApplyPageStyle(PageDescriptor page)
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.Background(ColorBackground);
            page.DefaultTextStyle(x => x
                .FontFamily(FontName)
                .FontSize(11)
                .FontColor(ColorTextPrimary));
        }

        private void SectionTitle(ColumnDescriptor column, string kicker, string title)
        {
            column.Item().Text(kicker.ToUpper())
                .FontSize(9)
                .LetterSpacing(0.08f)
                .Bold()
                .FontColor(ColorAccent);

            column.Item()
                .PaddingTop(2)
                .PaddingBottom(8)
                .BorderBottom(1)
                .BorderColor(ColorBorder)
                .Text(title)
                .FontSize(19)
                .Bold()
                .FontColor(ColorHeading);
        }

        private void PageFooterText(PageDescriptor page, string text)
        {
            page.Footer()
                .PaddingTop(10)
                .AlignCenter()
                .Text(text)
                .FontSize(9)
                .FontColor(ColorTextSecondary);
        }

        private void SimpleTableHeader(PageDescriptor page, string kicker, string title)
        {
            page.Header()
                .PaddingBottom(15)
                .BorderBottom(1)
                .BorderColor(ColorBorder)
                .Column(column =>
                {
                    column.Item().Text(kicker.ToUpper())
                        .FontSize(9)
                        .LetterSpacing(0.08f)
                        .Bold()
                        .FontColor(ColorAccent);

                    column.Item().PaddingTop(2).Text(title)
                        .FontSize(24)
                        .Bold()
                        .FontColor(ColorHeading);
                });
        }

        private void StatCard(TableDescriptor table, string label, string value, string caption = null)
        {
            table.Cell().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(14).Column(c =>
            {
                c.Item().Text(value).FontSize(20).Bold().FontColor(ColorHeading);
                c.Item().PaddingTop(4).Text(label.ToUpper()).FontSize(8).FontColor(ColorTextSecondary).Bold();

                if (!string.IsNullOrEmpty(caption))
                {
                    c.Item().PaddingTop(2).Text(caption).FontSize(9).FontColor(ColorAccent).Bold();
                }
            });
        }

        public async Task<string> SavePdfAsync(
           BusinessAnalyticsReport report,
           IWebHostEnvironment environment)
        {
            // إنشاء الـ PDF
            byte[] pdfBytes = GeneratePdf(report);

            // اسم الملف
            string fileName =
                    $"Report_{DateTime.Now.Year}_{DateTime.Now.Month:D2}.pdf";
            // المسار داخل wwwroot
            string folderPath = Path.Combine(
                environment.WebRootPath,
                "Reports",
                "Pdf");

            // لو الفولدر مش موجود اعمله
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // المسار الكامل للملف
            string filePath = Path.Combine(folderPath, fileName);

            // حفظ الملف
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            // المسار النسبي اللي هيتخزن في قاعدة البيانات
            return Path.Combine(
                "Reports",
                "Pdf",
                fileName).Replace("\\", "/");
        }

        public byte[] GeneratePdf(BusinessAnalyticsReport report)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                // ---------- Page 1: Summary ----------
                container.Page(page =>
                {
                    ApplyPageStyle(page);

                    page.Header()
                        .PaddingBottom(15)
                        .BorderBottom(1)
                        .BorderColor(ColorBorder)
                        .Column(column =>
                        {
                            column.Item().Text("ELIXIR")
                                .FontSize(30)
                                .Bold()
                                .FontColor(ColorHeading);

                            column.Item().PaddingTop(2).Text("Business Analytics Report")
                                .FontSize(16)
                                .FontColor(ColorTextPrimary);

                            column.Item().PaddingTop(6)
                                .Text($"AI-Powered Business Insights & Sales Forecast  •  Generated {DateTime.Now:dd MMMM yyyy}")
                                .FontSize(9)
                                .FontColor(ColorTextSecondary);
                        });

                    page.Content()
                        .PaddingTop(25)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            SectionTitle(column, "Overview", "Executive Summary");

                            column.Item().PaddingTop(6).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                StatCard(table, "Total Predicted Profit", $"{report.TotalPredictedProfit:N0} EGP");
                                StatCard(table, "Total Predicted Units", $"{report.TotalPredictedUnits:N0}");
                                StatCard(table, "Total Categories", $"{report.Categories.Count}");
                                StatCard(table, "Total Bottle Sizes", $"{report.Sizes.Count}");
                            });

                            column.Item().PaddingTop(6);

                            SectionTitle(column, "Highlights", "Product Performance");

                            column.Item().PaddingTop(6).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                // Best Performer
                                table.Cell().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(15).Column(c =>
                                {
                                    c.Item().Text("BEST PERFORMING PRODUCT").FontSize(8).Bold().FontColor(ColorAccent);
                                    c.Item().PaddingTop(4).Text(report.TopPerformer?.PerfumeName ?? "-").FontSize(15).Bold().FontColor(ColorHeading);
                                    c.Item().PaddingTop(6).Text($"Category: {report.TopPerformer?.CategoryName}").FontSize(10);
                                    c.Item().Text($"Gender: {report.TopPerformer?.Gender}").FontSize(10);
                                    c.Item().Text($"Predicted Units: {report.TopPerformer?.PredictedQuantity:N0}").FontSize(10);
                                    c.Item().PaddingTop(4).Text($"{report.TopPerformer?.PredictedProfit:N0} EGP predicted profit").FontSize(11).Bold().FontColor(ColorAccent);
                                });

                                // Lowest Performer
                                table.Cell().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(15).Column(c =>
                                {
                                    c.Item().Text("LOWEST PERFORMING PRODUCT").FontSize(8).Bold().FontColor(ColorAccent);
                                    c.Item().PaddingTop(4).Text(report.LowestPerformer?.PerfumeName ?? "-").FontSize(15).Bold().FontColor(ColorHeading);
                                    c.Item().PaddingTop(6).Text($"Category: {report.LowestPerformer?.CategoryName}").FontSize(10);
                                    c.Item().Text($"Gender: {report.LowestPerformer?.Gender}").FontSize(10);
                                    c.Item().Text($"Predicted Units: {report.LowestPerformer?.PredictedQuantity:N0}").FontSize(10);
                                    c.Item().PaddingTop(4).Text($"{report.LowestPerformer?.PredictedProfit:N0} EGP predicted profit").FontSize(11).Bold().FontColor(ColorAccent);
                                });
                            });
                        });

                    PageFooterText(page, "Generated by Elixir AI Business Analytics System");
                });

                // ---------- Page 2: Category Analytics ----------
                container.Page(page =>
                {
                    ApplyPageStyle(page);
                    SimpleTableHeader(page, "Breakdown", "Category Analytics");

                    page.Content().PaddingTop(20).Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).Text("CATEGORY").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("UNITS").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("PROFIT").FontSize(9).Bold().FontColor(ColorTextSecondary);
                            });

                            var index = 0;
                            foreach (var item in report.Categories)
                            {
                                var rowColor = index % 2 == 0 ? ColorPanel : ColorPanelAlt;

                                table.Cell().Background(rowColor).Padding(8).Text(item.CategoryName).FontColor(ColorHeading).Bold();
                                table.Cell().Background(rowColor).Padding(8).AlignRight().Text(item.TotalUnits.ToString("N0")).FontColor(ColorTextSecondary);
                                table.Cell().Background(rowColor).Padding(8).AlignRight().Text($"{item.TotalProfit:N0} EGP").Bold();

                                index++;
                            }
                        });
                    });

                    PageFooterText(page, "Category Performance Report");
                });

                // ---------- Page 3: Gender Analytics ----------
                container.Page(page =>
                {
                    ApplyPageStyle(page);
                    SimpleTableHeader(page, "Breakdown", "Gender Analytics");

                    page.Content().PaddingTop(20).Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).Text("GENDER").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("UNITS").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("PROFIT").FontSize(9).Bold().FontColor(ColorTextSecondary);
                            });

                            var index = 0;
                            foreach (var item in report.Genders)
                            {
                                var rowColor = index % 2 == 0 ? ColorPanel : ColorPanelAlt;

                                table.Cell().Background(rowColor).Padding(8).Text(item.Gender).FontColor(ColorHeading).Bold();
                                table.Cell().Background(rowColor).Padding(8).AlignRight().Text(item.TotalUnits.ToString("N0")).FontColor(ColorTextSecondary);
                                table.Cell().Background(rowColor).Padding(8).AlignRight().Text($"{item.TotalProfit:N0} EGP").Bold();

                                index++;
                            }
                        });
                    });

                    PageFooterText(page, "Gender Performance Report");
                });

                // ---------- Page 4: Bottle Size Analytics ----------
                container.Page(page =>
                {
                    ApplyPageStyle(page);
                    SimpleTableHeader(page, "Breakdown", "Bottle Size Analytics");

                    page.Content().PaddingTop(20).Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).Text("BOTTLE SIZE").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("UNITS").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("PROFIT").FontSize(9).Bold().FontColor(ColorTextSecondary);
                            });

                            var index = 0;
                            foreach (var item in report.Sizes)
                            {
                                var rowColor = index % 2 == 0 ? ColorPanel : ColorPanelAlt;

                                table.Cell().Background(rowColor).Padding(8).Text(item.Size).FontColor(ColorHeading).Bold();
                                table.Cell().Background(rowColor).Padding(8).AlignRight().Text(item.TotalUnits.ToString("N0")).FontColor(ColorTextSecondary);
                                table.Cell().Background(rowColor).Padding(8).AlignRight().Text($"{item.TotalProfit:N0} EGP").Bold();

                                index++;
                            }
                        });
                    });

                    PageFooterText(page, "Bottle Size Performance Report");
                });

                // ---------- Page 5: Top 10 Predicted Products ----------
                container.Page(page =>
                {
                    ApplyPageStyle(page);
                    SimpleTableHeader(page, "Ranking", "Top 10 Predicted Products");

                    page.Content().PaddingTop(20).Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);   // Rank
                                columns.RelativeColumn(3);    // Product
                                columns.RelativeColumn(2);    // Category
                                columns.RelativeColumn(1.5f); // Gender
                                columns.RelativeColumn(1.5f); // Units
                                columns.RelativeColumn(2);    // Profit
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).Text("#").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).Text("PRODUCT").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).Text("CATEGORY").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).Text("GENDER").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("UNITS").FontSize(9).Bold().FontColor(ColorTextSecondary);
                                header.Cell().BorderBottom(1).BorderColor(ColorBorder).Padding(6).AlignRight().Text("PROFIT").FontSize(9).Bold().FontColor(ColorTextSecondary);
                            });

                            var rank = 1;
                            foreach (var item in report.Top10Products)
                            {
                                var rowColor = rank % 2 != 0 ? ColorPanel : ColorPanelAlt;

                                table.Cell().Background(rowColor).Padding(6).Text(rank.ToString()).FontColor(ColorAccent).Bold();
                                table.Cell().Background(rowColor).Padding(6).Text(item.PerfumeName).FontColor(ColorHeading).Bold();
                                table.Cell().Background(rowColor).Padding(6).Text(item.CategoryName).FontColor(ColorTextSecondary);
                                table.Cell().Background(rowColor).Padding(6).Text(item.Gender).FontColor(ColorTextSecondary);
                                table.Cell().Background(rowColor).Padding(6).AlignRight().Text(item.PredictedQuantity.ToString("N0"));
                                table.Cell().Background(rowColor).Padding(6).AlignRight().Text($"{item.PredictedProfit:N0} EGP").Bold();

                                rank++;
                            }
                        });
                    });

                    PageFooterText(page, "Top Performing Products");
                });

                // ---------- Page 6: AI Insights ----------
                container.Page(page =>
                {
                    ApplyPageStyle(page);
                    SimpleTableHeader(page, "Summary", "AI Insights");

                    page.Content().PaddingTop(20).Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(15).Column(c =>
                        {
                            c.Item().Text("HIGHEST PREDICTED PROFIT PRODUCT").FontSize(8).Bold().FontColor(ColorAccent);
                            c.Item().PaddingTop(4).Text(report.TopPerformer?.PerfumeName ?? "-").FontSize(14).Bold().FontColor(ColorHeading);
                            c.Item().Text($"{report.TopPerformer?.PredictedProfit:N0} EGP").FontSize(11).Bold();
                        });

                        column.Item().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(15).Column(c =>
                        {
                            c.Item().Text("LOWEST PREDICTED PROFIT PRODUCT").FontSize(8).Bold().FontColor(ColorAccent);
                            c.Item().PaddingTop(4).Text(report.LowestPerformer?.PerfumeName ?? "-").FontSize(14).Bold().FontColor(ColorHeading);
                            c.Item().Text($"{report.LowestPerformer?.PredictedProfit:N0} EGP").FontSize(11).Bold();
                        });

                        column.Item().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(15).Column(c =>
                        {
                            c.Item().Text("BEST PERFORMING CATEGORY").FontSize(8).Bold().FontColor(ColorAccent);

                            var bestCategory = report.Categories
                                .OrderByDescending(x => x.TotalProfit)
                                .First();

                            c.Item().PaddingTop(4).Text(bestCategory.CategoryName).FontSize(14).Bold().FontColor(ColorHeading);
                            c.Item().Text($"{bestCategory.TotalProfit:N0} EGP").FontSize(11).Bold();
                        });

                        column.Item().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(15).Column(c =>
                        {
                            c.Item().Text("BEST PERFORMING BOTTLE SIZE").FontSize(8).Bold().FontColor(ColorAccent);

                            var bestSize = report.Sizes
                                .OrderByDescending(x => x.TotalProfit)
                                .First();

                            c.Item().PaddingTop(4).Text(bestSize.Size).FontSize(14).Bold().FontColor(ColorHeading);
                            c.Item().Text($"{bestSize.TotalProfit:N0} EGP").FontSize(11).Bold();
                        });

                        column.Item().Background(ColorPanel).Border(1).BorderColor(ColorBorder).Padding(15).Column(c =>
                        {
                            c.Item().Text("PREDICTION SUMMARY").FontSize(8).Bold().FontColor(ColorAccent);

                            c.Item().PaddingTop(6).Text(
                                $"The AI model analyzed {report.Top10Products.Count} top-performing products " +
                                $"with a total predicted profit of {report.TotalPredictedProfit:N0} EGP " +
                                $"and {report.TotalPredictedUnits:N0} expected units."
                            ).FontSize(10);
                        });
                    });

                    PageFooterText(page, "Generated by Elixir AI");
                });
            }).GeneratePdf();
        }
    }
}
