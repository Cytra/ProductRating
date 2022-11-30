using Amazon.DynamoDBv2;
using Infrastructure.Database;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Logger.Debug("init main");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
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
    //app.UseMiddleware<ExceptionMiddleware>();

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
