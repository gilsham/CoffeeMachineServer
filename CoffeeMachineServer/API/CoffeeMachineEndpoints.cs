using System.Diagnostics;
using CoffeeMachineServer.Interfaces;
using CoffeeMachineServer.Services;

namespace CoffeeMachineServer.API;

public static class CoffeeMachineEndpoints
{
    public static RouteGroupBuilder MapCoffeeMachineEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/brew-coffee", async context =>
        {
            var dateTimeService = context.RequestServices.GetService<IDateTimeService>()
                ?? throw new ApplicationException("Could not retrieve IDateTimeService");

            if (dateTimeService.GetNow() is { Month: (int)Month.April, Day: 1 })
            {
                context.Response.StatusCode = StatusCodes.Status418ImATeapot;
                return;
            }

            var coffeeMachine = context.RequestServices.GetService<ICoffeeMachineService>();
            if (coffeeMachine == null)
            {
                Debug.WriteLine("Could not retrieve ICoffeeMachineService");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }

            var coffeeResult = ICoffeeMachineService.Status.Unknown;

            try
            {
                coffeeResult = coffeeMachine.TryBrewCoffee();
            }
            catch (OpenWeatherApiService.OpenWeatherException ex)
            {
                Debug.WriteLine(ex);
            }

            switch (coffeeResult)
            {
                case ICoffeeMachineService.Status.Empty:
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    return;
                }

                case ICoffeeMachineService.Status.BrewingCold:
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsJsonAsync(
                        new CoffeeMachineStatus(
                            "Your refreshing iced coffee is ready",
                            DateTime.Now
                        ));
                    return;
                }

                case ICoffeeMachineService.Status.BrewingHot:
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsJsonAsync(
                        new CoffeeMachineStatus(
                            "Your piping hot coffee is ready",
                            DateTime.Now
                        ));
                    return;
                }

                case ICoffeeMachineService.Status.Ready:
                case ICoffeeMachineService.Status.Unknown:
                default:
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    Debug.WriteLine("TryBrewCoffee returned an unhandled status code");
                    return;
                }
            }
        });
        
        return builder;
    }
}