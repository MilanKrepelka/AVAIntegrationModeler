using MudBlazor.Services;
using AVAIntegrationModeler.Web.Components;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using AVAIntegrationModeler.Web.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Přidat lokalizaci
builder.Services.AddLocalization();

builder.Services.AddControllers(); // Add this line to support controllers

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// ✅ OPRAVENO - Konfigurace podporovaných kultur
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("cs")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("cs"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    // ✅ PŘIDÁNO - Důležité pro čtení z cookies!
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    }
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
