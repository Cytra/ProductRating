using Amazon.DynamoDBv2;
using Application.Options;
using Application.Ports;
using Application.Services;
using Infrastructure.Database;
using Infrastructure.Scrapers;
using Serilog;
using System.Net;
using Application.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using ProductRating.Middleware;
using ProductRating.Models;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Logger.Debug("init main");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.

    builder.Services.Configure<AppOptions>(builder.Configuration);

    builder.Services.AddScoped<IAmazonScrapper, AmazonScrapper>();

    builder.Services.AddScoped<ISeleniumDriverFactory, SeleniumDriverFactory>();

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
                        apiResponse.Errors.Add(new Error() { ErrorCode = (int)ErrorCodes.BadRequest, ErrorMessage = error.ErrorMessage });
                    }
                }
                return new BadRequestObjectResult(apiResponse);
            };
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddTransient<IAmazonDynamoDB>(_ =>
    {
        var clientConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = builder.Configuration.GetValue<string>("Dynamodb-endpoint-url")
        };
        return new AmazonDynamoDBClient(clientConfig);
    });

    var app = builder.Build();

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
