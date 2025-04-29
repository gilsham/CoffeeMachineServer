using CoffeeMachineServer.Interfaces;

namespace CoffeeMachineServer.Services;

public class DateTimeService :  IDateTimeService
{
    public DateTime GetNow()
        => DateTime.Now;
}