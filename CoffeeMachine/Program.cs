var builder = WebApplication.CreateBuilder(args);
var coffeeMachine = new CoffeeMachine();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/brew-coffee", () =>
    {
        return coffeeMachine.TryBrewCoffee() switch
        {
            CoffeeMachine.Status.Brewing => Results.StatusCode(StatusCodes.Status503ServiceUnavailable),
            CoffeeMachine.Status.Empty => Results.Ok(
                    new CoffeeMachineStatus("Your piping hot coffee is ready",
                    DateTime.Now
                )),
            _ => Results.InternalServerError()
        };
    })
    .WithName("GetBrewCoffee");

app.Run();

internal record CoffeeMachineStatus(string Message, DateTime Prepared);

internal class CoffeeMachine
{
    private int _brewCount = 0;
    public enum Status
    {
        Ready,
        Brewing,
        Empty,
    }
    public Status TryBrewCoffee()
    {
        _brewCount++;
        
        return _brewCount % 5 is 0 
            ? Status.Empty 
            : Status.Brewing;
    }
}