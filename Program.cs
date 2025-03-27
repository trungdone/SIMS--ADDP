using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SIMS_App.Data;
using SIMS_App.Services;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký các dịch vụ cần thiết
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();


// Đăng ký IUserService

// Đăng ký IDataService
builder.Services.AddScoped<IDataService, DataService>();




builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // HTTP
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Auth}/{action=Login}/{id?}");
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=DashboardAdmin}/{id?}");
});


app.Run();






