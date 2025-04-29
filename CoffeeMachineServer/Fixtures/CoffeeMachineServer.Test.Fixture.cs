using CoffeeMachineServer.Interfaces;
using CoffeeMachineServer.Services;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace CoffeeMachineServer.Fixtures;

public class CoffeeMachineServerTestFixture : TestBedFixture
{
    public TestWeatherApiService TestWeatherApiService = new TestWeatherApiService();
    
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    {
        services.AddTransient<ICoffeeMachineService, CoffeeMachineService>();
        services.AddTransient<IWeatherApiService>(_ => TestWeatherApiService);
    }

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new TestAppSettings();
    }

    protected override ValueTask DisposeAsyncCore()
        => new();
}