using Geonorge.OpplastingsApi.Models.Entity;
using Geonorge.OpplastingsApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


    

