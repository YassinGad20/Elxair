using Elixir.Services;
using Elxair.Models;
using Elxair.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ElxairContext>(options =>
    options.UseSqlServer("Server=.;Database=Elxair;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<BusinessAnalyticsService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<JsonReportService>();
builder.Services.AddScoped<RagService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseMiddleware<Elxair.Middleware.GuestMiddleware>();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var pythonProcess = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = @"D:\Fci 2025-2026\Software Development\Elxair_Project\Elxair\Model\FastMl\Scripts\python.exe",

        Arguments = "-m uvicorn app:app --host 127.0.0.1 --port 8000",

        WorkingDirectory = @"D:\Fci 2025-2026\Software Development\Elxair_Project\Elxair\Model",

        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    }
};

pythonProcess.OutputDataReceived += (s, e) =>
{
    if (!string.IsNullOrWhiteSpace(e.Data))
        Console.WriteLine(e.Data);
};

pythonProcess.ErrorDataReceived += (s, e) =>
{
    if (!string.IsNullOrWhiteSpace(e.Data))
        Console.WriteLine(e.Data);
};

pythonProcess.Start();
pythonProcess.BeginOutputReadLine();
pythonProcess.BeginErrorReadLine();

// Give FastAPI time to start
await Task.Delay(3000);



app.Run();