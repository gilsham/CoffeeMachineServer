using CoffeeMachineServer.Interfaces;

namespace CoffeeMachineServer.Services;

public class CoffeeMachineService(IWeatherApiService weatherApiService) : ICoffeeMachineService
{
    private int _brewCount = 0;

    public ICoffeeMachineService.Status TryBrewCoffee()
    {
        _brewCount++;
    
        if( _brewCount % 5 is 0 ) 
            return  ICoffeeMachineService.Status.Empty;
        
        return weatherApiService.GetCurrentTemperature() > 30.0 
            ? ICoffeeMachineService.Status.BrewingCold
            : ICoffeeMachineService.Status.BrewingHot;
    }
}