using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Exampler_ERP.Language;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext and services
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"))
 );
builder.Services.AddSingleton<IStringLocalizerFactory, DbStringLanguageFactory>();

builder.Services.AddScoped<Utils>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSession(options =>
{
  options.Cookie.HttpOnly = true; 
  options.Cookie.IsEssential = true; 
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var defaultCulture = builder.Configuration.GetValue<string>("AppSettings:DefaultCulture") ?? "en-US";
var supportedCultures = new[] { "en-US", "ur-PK" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(defaultCulture)
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

builder.Services.Configure<FormOptions>(options =>
{
  options.ValueCountLimit = int.MaxValue; // For many form fields
  options.MultipartBodyLengthLimit = 100_000_000; // 100 MB
});
builder.WebHost.ConfigureKestrel(options =>
{
  options.Limits.MaxRequestBodySize = 100_000_000; // 100 MB
});
builder.Services.Configure<FormOptions>(options =>
{
  options.ValueCountLimit = int.MaxValue;         // from default 1024
  options.ValueLengthLimit = int.MaxValue;
  options.MultipartBodyLengthLimit = long.MaxValue;
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
  options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseRequestLocalization(localizationOptions);
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseRequestLocalization(localizationOptions);
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
