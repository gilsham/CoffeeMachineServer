using System.Globalization;
using System.Text.Json;
using CoffeeMachineServer.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CoffeeMachineServer.Services;

public class OpenWeatherApiService(
    HttpClient httpHttpClient, 
    IOptions<OpenWeatherApiService.APIOptions> options
) : IWeatherApiService {
    private float? _currentTemperature;
    private DateTime? _lastUpdate;
    
    private readonly APIOptions _options = options.Value;
    
    private Task<OpenWeatherResponse> CallApi()
    {
        var urlParams = new Dictionary<string, string?> {
            ["lat"] = _options.Lat.ToString(CultureInfo.CurrentCulture),
            ["lon"] = _options.Lon.ToString(CultureInfo.CurrentCulture),
            ["appid"] = _options.ApiKey,
            ["units"] = "metric",
        };
        
        var response = httpHttpClient.GetAsync(QueryHelpers.AddQueryString(_options.ApiEndpoint, urlParams));
        response.Wait();
        
        if (response.IsCompletedSuccessfully)
            return response.Result.Content.ReadFromJsonAsync<OpenWeatherResponse>(JsonSerializerOptions.Web);

        throw new OpenWeatherException($"Failed to retrieve weather:  ${response.Result.ReasonPhrase}");
    }

    private bool UpdateTemp()
    {
        var weather = CallApi();
        weather.Wait();

        _currentTemperature = weather.Result.Main.Temp;
        _lastUpdate = DateTime.Now;
        return true;
    }

    public float GetCurrentTemperature()
    {
        if (_currentTemperature.HasValue
            && _lastUpdate.HasValue 
            && DateTime.Now.Subtract(_lastUpdate.Value).Hours < 1
        )
            return _currentTemperature.Value;
        
        if (UpdateTemp())
        {
            return _currentTemperature!.Value;
        }
        
        throw new OpenWeatherException("Unable to update temperature");
    }

    // Api Docs https://openweathermap.org/current
    internal struct OpenWeatherResponse
    {
        internal CurrentWeather Main { get; init; }
    }

    internal struct CurrentWeather
    {
        internal float Temp { get; init; }
    }
    
    public class APIOptions
    {
        public required float Lat { get; init; }
        public required float Lon { get; init; }
        public required string ApiKey { get; init; }
        public required string ApiEndpoint { get; init; }
    }

    public class OpenWeatherException : Exception
    {
        public OpenWeatherException(string s)
        {
            throw new Exception(s);
        }
    }

}