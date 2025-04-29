using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CoffeeMachineServer.Extensions;
using CoffeeMachineServer.Fixtures;
using CoffeeMachineServer.Interfaces;
using FluentAssertions;
using Xunit;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using ITestOutputHelper = Xunit.Abstractions.ITestOutputHelper;

namespace CoffeeMachineServer;

public class TestWeatherApiService : IWeatherApiService
{
    public float CurrentTemperature { get; set; } = 20;

    public float GetCurrentTemperature()
    {
        return CurrentTemperature;
    }
}

public class CoffeeMachineServerTest : TestBed<CoffeeMachineServerTestFixture>
{
    private readonly ICoffeeMachineService _coffeeMachineService;
    private readonly HttpClient _httpClient;

    public CoffeeMachineServerTest(ITestOutputHelper testOutputHelper, CoffeeMachineServerTestFixture fixture)
        : base(testOutputHelper, fixture)
    {
        _httpClient = new HttpClient();
        
        _coffeeMachineService = _fixture.GetService<ICoffeeMachineService>(_testOutputHelper)
                                ?? throw new Exception("No ICoffeeMachineService  was registered");
    }

    [Fact]
    public void TryBrewCoffee_FiveTimes()
    {
        foreach (var _ in Enumerable.Range(1, 4))
        {
            _coffeeMachineService.TryBrewCoffee();
        }
        
        _coffeeMachineService.TryBrewCoffee().Should().Be(ICoffeeMachineService.Status.Empty);
    }

    [Fact]
    public void TryBrewCoffee_InHotWeather()
    {
        _fixture.TestWeatherApiService.CurrentTemperature = 35;
        
        _coffeeMachineService.TryBrewCoffee().Should().Be(ICoffeeMachineService.Status.BrewingCold);
    }
    
    [Fact]
    public void TryBrewCoffee_InColdWeather()
    {
        _fixture.TestWeatherApiService.CurrentTemperature = 15;
        
        _coffeeMachineService.TryBrewCoffee().Should().Be(ICoffeeMachineService.Status.BrewingHot);
    }
}

public class CoffeeMachineEndpointsTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;

    public CoffeeMachineEndpointsTests(TestWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task TryBrewCoffee_OnApril1st()
    {
        _factory.DateTimeService.Now = new DateTime(2000, (int)Month.April, 1);
        var result = await _httpClient.GetAsync("/brew-coffee");
        
        result.StatusCode.Should().Be((HttpStatusCode)418);
        _factory.DateTimeService.Now = DateTime.Now;
    }
    
    [Theory]
    [InlineData(ICoffeeMachineService.Status.BrewingHot, "Your piping hot coffee is ready")]
    [InlineData(ICoffeeMachineService.Status.BrewingCold, "Your refreshing iced coffee is ready")]
    public async Task TryBrewCoffee_WithMessage(ICoffeeMachineService.Status status, string expectedMessage)
    {
        _factory.CoffeeMachineService.outputStatus = status;
        
        var result = await _httpClient.GetAsync("/brew-coffee");

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadFromJsonAsync<CoffeeMachineStatus>();
        content.Should().NotBeNull();
        content.Message.Should().Be(expectedMessage);
    }
    
    [Theory]
    [InlineData(ICoffeeMachineService.Status.Empty, HttpStatusCode.ServiceUnavailable)]
    [InlineData(ICoffeeMachineService.Status.Ready, HttpStatusCode.InternalServerError)]
    [InlineData(ICoffeeMachineService.Status.Unknown, HttpStatusCode.InternalServerError)]
    public async Task TryBrewCoffee_WithoutMessage(ICoffeeMachineService.Status status, HttpStatusCode expectedStatus)
    {
        _factory.CoffeeMachineService.outputStatus = status;
        
        var result = await _httpClient.GetAsync("/brew-coffee");

        result.StatusCode.Should().Be(expectedStatus);
    }
    
    [Fact]
    public async Task TryRandomEndpoint()
    {
        var endpoint = GetUniqueKey();
        _testOutputHelper.WriteLine($"Testing random endpoint: {endpoint}");
        var result = await _httpClient.GetAsync($"/{endpoint}");
        
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static readonly char[] Chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    private static string GetUniqueKey()
    {
        using var crypto = RandomNumberGenerator.Create();

        var length = Enumerable.Range(1, 100).SelectRandom();
        var data = new byte[4 * length];
        crypto.GetBytes(data);

        var result = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % Chars.Length;

            result.Append(Chars[idx]);
        }

        return result.ToString();
    }
}