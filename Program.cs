using System.Net;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

// TODO: Potentially convert this back to using the Program -> Main structure
// namespace rest_api;

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.Console()
.WriteTo.Async(a => a.File(new JsonFormatter(renderMessage: true), "logs/rest_api_.log", rollingInterval: RollingInterval.Day))
.CreateLogger();
try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();
    builder.Services.AddSingleton<DapperContext>();
    builder.Services.AddControllersWithViews();
    builder.Services.AddRouting(options =>
    {
        options.LowercaseUrls = true;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Some API",
            Description = "Some description",
        });
    });

    var app = builder.Build();
    // wwwroot
    app.UseDefaultFiles();
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        Log.Debug("Develop environment, enabling Swapper & OpenAPI");
        app.UseSwagger();
        // Expose swagger/openapi ui at root route
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "swagger";
        });
    }

    // Log.Debug("Adding /healthcheck endpoint", HealthCheck);
    // app.MapGet("/healthcheck", HealthCheck);

    // Log.Debug("Constructing /api group");
    // var api = app.MapGroup("/api");
    // api.MapGet("hello", () => "Hello!");

    // Log.Debug("/api endpoints created");


    app.MapControllers();

    await app.RunAsync();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}


static async Task<HttpResponseMessage> HealthCheck()
{
    // TODO: add checks and information from future subsystems as they're added.
    var response = new HttpResponseMessage(HttpStatusCode.OK);

    return response;
}

