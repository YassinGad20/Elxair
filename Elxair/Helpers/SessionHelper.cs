using Microsoft.AspNetCore.Http;

public static class SessionHelper
{
    public static int? GetUserId(HttpContext context) =>
        context.Session.GetInt32("UserId");

    public static string GetUserRole(HttpContext context) =>
        context.Session.GetString("UserRole");

    public static string GetUserName(HttpContext context) =>
        context.Session.GetString("UserName");
}
