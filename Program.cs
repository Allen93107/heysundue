using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Heysundue.Data;
using Heysundue.Models;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 添加会话服务
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 设置会话超时时间
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add DbContext and ConfigureServices
builder.Services.AddDbContext<ArticleContext>(options =>
{
    if (env.IsDevelopment())
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("ArticleContext"));
    }
    else
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("ArticleContext"));
    }
});



builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!env.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 使用会话中间件
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index2}/{id?}");

app.MapRazorPages();

app.Run();
