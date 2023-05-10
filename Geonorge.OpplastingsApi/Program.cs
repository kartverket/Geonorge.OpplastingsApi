using Geonorge.OpplastingsApi.Extensions;
using Geonorge.OpplastingsApi.Models.Entity;
using Geonorge.OpplastingsApi.Services;
using LoggingWithSerilog.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Fyll inn \"Bearer\" [space] og en token i tekstfeltet under. Eksempel: \"Bearer b990274d-2082-34a5-9768-02b396f98d8d\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IDatasetService, DatasetService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddDbContext<ApplicationContext>(opts =>
        opts.UseSqlServer(builder.Configuration.GetConnectionString("UploadApiDatabase")));

builder.Services.Configure<AuthConfiguration>(configuration.GetSection(AuthConfiguration.SectionName));
builder.Services.Configure<NotificationConfiguration>(configuration.GetSection(NotificationConfiguration.SectionName));

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


    

