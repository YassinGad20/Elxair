using Elxair.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// 1. ≈÷«›… Œœ„«  «·‹ Session Ê«·‹ HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // „œ… «·Ã·”…
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



//  ”ÃÌ· «·”Ì—›Ì”“ ⁄‘«‰ ‰ﬁœ— ‰” Œœ„Â„
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseSession(); // „Â„ Ãœ« ﬁ»· «·‹ Routing
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();