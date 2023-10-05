using BlazorAppRollbarDemo.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rollbar;
using Rollbar.NetCore.AspNet;

var builder = WebApplication.CreateBuilder(args);

// Rollbar Steps
// STEP.1 - enable Http Context Accessor:
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// STEP.2 - Setup Rollbar Infrastructure:
ConfigureRollbarInfrastructure();

// STEP.3 - Add Rollbar logger with properly configured log level filter:
builder.Services.AddRollbarLogger(loggerOptions =>
{
    loggerOptions.Filter =
      (loggerName, loglevel) => loglevel >= LogLevel.Trace;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// STEP.4 - Use Rollbar middleware:
app.UseRollbarMiddleware();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();



// STEP.2 - Setup Rollbar Infrastructure:
void ConfigureRollbarInfrastructure()
{
    RollbarInfrastructureConfig config = new RollbarInfrastructureConfig(
      "your_access_token_here",
      "production"
    );
    RollbarDataSecurityOptions dataSecurityOptions = new RollbarDataSecurityOptions();
    dataSecurityOptions.ScrubFields = new string[]
    {
      "url",
      "method",
    };
    config.RollbarLoggerConfig.RollbarDataSecurityOptions.Reconfigure(dataSecurityOptions);

    RollbarInfrastructure.Instance.Init(config);

    //// Optionally:
    //RollbarInfrastructure.Instance.QueueController.InternalEvent += OnRollbarInternalEvent;
}
