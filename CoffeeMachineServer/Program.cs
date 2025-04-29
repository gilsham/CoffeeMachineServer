using CoffeeMachineServer.API;
using CoffeeMachineServer.Interfaces;
using CoffeeMachineServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var options = builder.Configuration.GetSection("OpenWeatherAPIOptions");
builder.Services.Configure<OpenWeatherApiService.APIOptions>(options);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWeatherApiService, OpenWeatherApiService>();
builder.Services.AddSingleton<ICoffeeMachineService, CoffeeMachineService>();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGroup("")
    .MapCoffeeMachineEndpoints()
    .WithName("CoffeeMachine");

app.UseHttpsRedirection();

app.Run();

public partial class Program { }

internal record CoffeeMachineStatus(string Message, DateTime Prepared);