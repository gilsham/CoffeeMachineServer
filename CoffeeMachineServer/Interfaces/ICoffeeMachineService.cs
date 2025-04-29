namespace CoffeeMachineServer.Interfaces;

public interface ICoffeeMachineService
{
    public enum Status
    {
        Unknown,
        Ready,
        BrewingHot,
        BrewingCold,
        Empty,
    }

    public Status TryBrewCoffee();
}