using System.Text.Json;
using Elxair.Models;
using Microsoft.AspNetCore.Hosting;

namespace Elixir.Services
{
    public class JsonReportService
    {

        public string GenerateJson(BusinessAnalyticsReport report)
        {
            return JsonSerializer.Serialize(
                report,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
        public async Task<string> SaveJsonAsync(
            BusinessAnalyticsReport report,
            IWebHostEnvironment environment)
        {
            // تحويل الـ Report إلى JSON
            string json = JsonSerializer.Serialize(
                report,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            // إنشاء اسم الملف
            string fileName =
                $"Report_{DateTime.Now.Year}_{DateTime.Now.Month:D2}.json";
            // إنشاء مسار الحفظ
            string folderPath = Path.Combine(
                environment.WebRootPath,
                "Reports",
                "Json");

            // إنشاء الفولدر إذا لم يكن موجوداً
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // المسار الكامل للملف
            string filePath = Path.Combine(folderPath, fileName);

            // حفظ الملف
            await File.WriteAllTextAsync(filePath, json);

            // يرجع المسار النسبي الذى سيتم حفظه فى قاعدة البيانات
            return Path.Combine(
                "Reports",
                "Json",
                fileName).Replace("\\", "/");
        }
    }
}