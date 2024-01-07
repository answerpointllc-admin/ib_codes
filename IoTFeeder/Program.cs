using IoTFeeder.Common.DB;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(prefix: "/")
    .Build();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

#region Dependency Injection
builder.Services.AddDbContext<IotDataFeederContext>(option => option.UseSqlServer(config.GetConnectionString("IoTFeeder_Connection")));
builder.Services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IIoTDevice, IoTDeviceRepository>();
builder.Services.AddScoped<IIoTDeviceProperty, IoTDevicePropertyRepository>();
builder.Services.AddScoped<ICommonSettings, CommonSettingsReepository>();
builder.Services.AddKendo();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        context.Request.Path = "/error/404";
        await next();
    }
    else if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
    {
        context.Request.Path = "/error/500";
        await next();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
