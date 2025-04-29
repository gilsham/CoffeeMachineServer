using CoffeeMachineServer.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CoffeeMachineServer.Fixtures;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public TestCoffeeMachineService CoffeeMachineService { get; set; } = new();
    public TestDateTimeService DateTimeService { get; set; } = new();
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddTransient<ICoffeeMachineService>(_ => CoffeeMachineService);
            services.AddTransient<IDateTimeService>(_ => DateTimeService);
        });
        
        return base.CreateHost(builder);
    }

    public class TestCoffeeMachineService : ICoffeeMachineService
    {
        public ICoffeeMachineService.Status outputStatus { set; get; }
        public ICoffeeMachineService.Status TryBrewCoffee()
        {
            return outputStatus;
        }
    }
    
    public class TestDateTimeService : IDateTimeService
    {
        public DateTime Now { set; get; } = DateTime.Now;
        
        public DateTime GetNow()
        {
            return Now;
        }
    }
}