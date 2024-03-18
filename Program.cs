using Microsoft.EntityFrameworkCore;
using MoscowWeather.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
if (connection.Contains("%CONTENTROOTPATH%"))
{
    string rootPath = builder.Environment.ContentRootPath.ToString();
    if (builder.Environment.ContentRootPath.ToString().Last() != '\\')
        rootPath += "\\";
    connection = connection.Replace("%CONTENTROOTPATH%", rootPath);
}

builder.Services.AddDbContext<WeatherContext>(options => options.UseSqlServer(connection));
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
