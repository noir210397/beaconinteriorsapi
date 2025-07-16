using beaconinteriorsapi.Data;
using beaconinteriorsapi.DependencyInjection;
using beaconinteriorsapi.Filters;
using beaconinteriorsapi.Services;
using dotenv.net;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
ValidatorOptions.Global.DefaultRuleLevelCascadeMode=CascadeMode.Stop;

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("https://localhost:3000").AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddDbContext<BeaconInteriorsDBContext>(options =>  options.UseSqlServer(builder.Configuration.GetConnectionString("BeaconInteriorsDBConnection")));
builder.Services.AddAppServices();
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var appService = scope.ServiceProvider.GetRequiredService<AppStartupService>();
    await appService.Initialize();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
