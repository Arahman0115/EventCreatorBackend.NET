using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class GeocodingService
{
    private readonly HttpClient _httpClient;
    private static readonly SemaphoreSlim _throttler = new SemaphoreSlim(1);
    private static DateTime _lastRequestTime = DateTime.MinValue;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        
        // Add a User-Agent header - this is required by Nominatim
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ProtestMapAPI/1.0 (your@email.com)");
        
        // Also good practice to add accept header
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    }

    public async Task<(double, double)> GetCoordinatesAsync(string street, string city, string state, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(zipCode))
        {
            return (0, 0); // Return default if any parameter is missing
        }

        var address = $"{street}, {city}, {state} {zipCode}";
        var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

        await _throttler.WaitAsync();
        try
        {
            // Ensure at least 1 second between requests (to respect rate limits)
            var timeSinceLastRequest = DateTime.Now - _lastRequestTime;
            if (timeSinceLastRequest.TotalSeconds < 1)
            {
                await Task.Delay(1000 - (int)timeSinceLastRequest.TotalMilliseconds);
            }
            _lastRequestTime = DateTime.Now;

            var response = await _httpClient.GetStringAsync(url);
            var json = JArray.Parse(response);

            if (json.Count > 0)
            {
                var firstResult = json[0];

                // Check if properties exist and are valid before parsing
                if (firstResult["lat"]?.ToObject<double>() is double lat &&
                    firstResult["lon"]?.ToObject<double>() is double lon)
                {
                    return (lat, lon);
                }
            }
        }
        catch (HttpRequestException)
        {
            // Network-related issue; log it if necessary
        }
        catch (Exception)
        {
            // General failure; log it if needed
        }
        finally
        {
            _throttler.Release();
        }

        return (0, 0); // Return default coordinates if geocoding fails
    }
}
