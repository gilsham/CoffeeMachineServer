namespace CoffeeMachineServer.Interfaces;

public class CoffeeMachineInterface
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