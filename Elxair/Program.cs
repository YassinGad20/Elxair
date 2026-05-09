using Elxair.Models;
using Elxair.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. HttpContext
builder.Services.AddHttpContextAccessor();

// 2. Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Database
builder.Services.AddDbContext<ElxairContext>(options =>
    options.UseSqlServer("Server=.;Database=Elxair;Trusted_Connection=True;TrustServerCertificate=True;"));

// 4. Services (DI)
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<PaymentService>();

//AiService
builder.Services.AddScoped<AiService>();

// 5. MVC
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();// call api

var app = builder.Build();

// 6. Middleware
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // لازم بعد UseRouting وقبل Authorization

app.UseAuthorization();

// 7. Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();