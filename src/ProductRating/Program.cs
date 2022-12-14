using Application.Options;
using Application.Ports;
using Application.Services;
using Infrastructure.Scrapers;
using Serilog;
using System.Net;
using Application.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using ProductRating.Middleware;
using Application.Facades;
using MediatR;
using System.Reflection;
using Application.Commands;
using Application.Models;
using Elastic.CommonSchema.Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .WriteTo
    .File(new EcsTextFormatter(), "/path/to/log.txt")
    .CreateLogger();

try
{
    Log.Logger.Debug("init main");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.

    builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    builder.Services.AddMediatR(typeof(ProductsBySearchTerm.Command).Assembly);

    builder.Services.Configure<AppOptions>(builder.Configuration);

    builder.Services.AddScoped<IAmazonScrapper, AmazonScrapper>();

    builder.Services.AddScoped<IAmazonScrapperFacade, AmazonScrapperFacade>();

    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, CacheService>();

    builder.Services.AddHttpClient<IAmazonHttpClient, AmazonHttpClient>()
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    });

    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(setupAction =>
        {
            setupAction.InvalidModelStateResponseFactory = context =>
            {
                var apiResponse = new ErrorResponse();
                foreach (var modelState in context.ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        apiResponse.Errors.Add(new Error() { ErrorCode = ErrorCodes.BadRequest, ErrorMessage = error.ErrorMessage });
                    }
                }
                return new BadRequestObjectResult(apiResponse);
            };
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    app.UseHttpLogging();
    var env = app.Environment;
    if (env.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
        });
    }

    app.UseDeveloperExceptionPage();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/healthz");
    });

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Stopped program because of exception");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
