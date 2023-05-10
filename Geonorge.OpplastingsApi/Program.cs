using Geonorge.OpplastingsApi.Extensions;
using Geonorge.OpplastingsApi.Models.Entity;
using Geonorge.OpplastingsApi.Services;
using LoggingWithSerilog.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

string configFile = "appsettings.json";
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
    configFile = "appsettings.Development.json";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(configFile)
    .Build();

var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IDatasetService, DatasetService>();
builder.Services.AddDbContext<ApplicationContext>(opts =>
        opts.UseSqlServer(builder.Configuration.GetConnectionString("UploadApiDatabase")));

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(swagger =>
{    
    var url = $"{(!app.Environment.IsLocal() ? "/api" : "")}/swagger/v1/swagger.json";
    swagger.SwaggerEndpoint(url, "Geonorge.OpplastingsApi API V1");
});

app.UseHttpsRedirection();

app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

app.UseAuthorization();

app.MapControllers();

var urlProxy = configuration.GetValue<string>("UrlProxy");

if (!string.IsNullOrWhiteSpace(urlProxy))
{
    var proxy = new WebProxy(urlProxy) { Credentials = CredentialCache.DefaultCredentials };
    WebRequest.DefaultWebProxy = proxy;
    HttpClient.DefaultProxy = proxy;
}

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    dataContext.Database.Migrate();
}

app.Run();


    

