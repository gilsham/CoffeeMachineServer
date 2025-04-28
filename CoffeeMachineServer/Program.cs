using CoffeeMachineServer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<CoffeeMachineInterface>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/brew-coffee", () =>
    {
        if (DateTime.Today.Month == (int)Month.April && DateTime.Today.Day == 1)
            return Results.StatusCode(StatusCodes.Status418ImATeapot);
        
        var coffeeMachine = app.Services.GetService<CoffeeMachineInterface>();
        if (coffeeMachine == null)
            return Results.InternalServerError();

        return coffeeMachine.TryBrewCoffee() switch
        {
            CoffeeMachineInterface.Status.Empty
                => Results.StatusCode(StatusCodes.Status503ServiceUnavailable),
            
            CoffeeMachineInterface.Status.Brewing
                => Results.Ok(
                    new CoffeeMachineStatus(
                        "Your piping hot coffee is ready",
                        DateTime.Now
                    )),
            
            _ => Results.InternalServerError()
        };
    })
    .WithName("GetBrewCoffee");

app.Run();

internal record CoffeeMachineStatus(string Message, DateTime Prepared);