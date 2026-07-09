using UmaFestHub.Application.Extensions;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Infrastructure.Extensions;
using UmaFestHub.Web.Hubs;
using UmaFestHub.Web.Services;
using UmaFestHub.Application.Validation;
using UmaFestHub.Web.Options;
using UmaFestHub.Web.Validation.Validators;
using UmaFestHub.Web.Workers;
using UmaFestHub.Web.ViewModels;  
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

using System.Globalization;
using Microsoft.AspNetCore.Localization;
using UmaFestHub.Web.Extensions;
using UmaFestHub.Web.Resources;

var envCandidates = new[]
{
    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../.env")),
    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.env")),
    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".env")),
    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../.env"))
};

var envPath = envCandidates.FirstOrDefault(File.Exists);
if (!string.IsNullOrWhiteSpace(envPath))
{
    Env.Load(envPath);
}
else
{
    Env.Load();
}

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TMDB_API_KEY"))
    && !string.IsNullOrWhiteSpace(envPath)
    && File.Exists(envPath))
{
    var tmdbLine = File.ReadLines(envPath)
        .FirstOrDefault(l => l.StartsWith("TMDB_API_KEY=", StringComparison.Ordinal));

    if (!string.IsNullOrWhiteSpace(tmdbLine))
    {
        var tmdbKey = tmdbLine["TMDB_API_KEY=".Length..].Trim();
        if (!string.IsNullOrWhiteSpace(tmdbKey))
        {
            Environment.SetEnvironmentVariable("TMDB_API_KEY", tmdbKey);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
// In-app notifications: Web implementation (SignalR hub context + queued persistence).
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationTemplateRenderer, NotificationTemplateRenderer>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieSignInService, CookieSignInService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IViewModelValidator<EditProfileViewModel>, EditProfileViewModelValidator>();
builder.Services.AddInfrastructure(builder.Configuration);
// Festival ending + rental access reminders: bind schedules from appsettings; BackgroundService passes invoke reminder services.
// Purchase-completed notification is immediate from PurchaseService (not hosted here).
builder.Services.Configure<FestivalEndingReminderOptions>(
	builder.Configuration.GetSection(FestivalEndingReminderOptions.SectionKey));
builder.Services.AddHostedService<FestivalEndingReminderWorker>();
builder.Services.Configure<RentalExpiryReminderOptions>(
	builder.Configuration.GetSection(RentalExpiryReminderOptions.SectionKey));
builder.Services.AddHostedService<RentalExpiryReminderWorker>();
builder.Services.Configure<AwardExpiryOptions>(
	builder.Configuration.GetSection(AwardExpiryOptions.SectionKey));
builder.Services.AddHostedService<AwardExpiryWorker>();
builder.Services.AddLocalization();

builder.Services.AddControllersWithViews()
	.AddViewLocalization()
	.AddDataAnnotationsLocalization(options =>
	{
		options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResources));
	});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var supportedCultures = new[] { "en", "pt", "fr" };
	options.SetDefaultCulture("en")
		.AddSupportedCultures(supportedCultures)
		.AddSupportedUICultures(supportedCultures);

	options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                context.Response.Redirect("/?toast=access-denied");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddHttpClient<ITmdbClient, TmdbClient>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

var app = builder.Build();

//  Run migrations and seed FIRST before anything else
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UmaFestHub.Infrastructure.Data.AppDbContext>();
    await db.Database.MigrateAsync();
    await UmaFestHub.Infrastructure.Seed.AdminSeeder.SeedAsync(db);
    // await UmaFestHub.Infrastructure.Seed.SessionSeeder.SeedAsync(db);
    await UmaFestHub.Infrastructure.Seed.ProductSeeder.SeedAsync(db);

}

app.UseExceptionHandler("/Error");
app.UseStaticFiles();

var localizationOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);
app.Use(async (context, next) =>
{
	var regional = LocalizationExtensions.MapToRegionalCulture(CultureInfo.CurrentUICulture);
	CultureInfo.CurrentCulture = regional;
	CultureInfo.CurrentUICulture = regional;
	await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// In-app notifications: browser connects here; NotificationHub places connections in user/role groups.
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<WatchPartyHub>("/watchPartyHub");
app.MapControllers();
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();