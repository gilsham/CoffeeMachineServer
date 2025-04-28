using CoffeeMachineServer.Interfaces;
using Xunit;
using FluentAssertions;

namespace CoffeeMachineServer;

public class CoffeeMachineServerTest
{
    [Fact]
    public void TryBrewCoffee()
    {
        var testMachine = new CoffeeMachineInterface();

        testMachine.TryBrewCoffee().Should().Be(CoffeeMachineInterface.Status.Brewing);
    }

    [Fact]
    public void TryBrewCoffee_FiveTimes()
    {
        var testMachine = new CoffeeMachineInterface();

        testMachine.TryBrewCoffee().Should().Be(CoffeeMachineInterface.Status.Brewing);
        testMachine.TryBrewCoffee().Should().Be(CoffeeMachineInterface.Status.Brewing);
        testMachine.TryBrewCoffee().Should().Be(CoffeeMachineInterface.Status.Brewing);
        testMachine.TryBrewCoffee().Should().Be(CoffeeMachineInterface.Status.Brewing);
        testMachine.TryBrewCoffee().Should().Be(CoffeeMachineInterface.Status.Empty);
    }
}