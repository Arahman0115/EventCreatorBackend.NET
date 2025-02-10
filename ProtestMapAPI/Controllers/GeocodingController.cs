using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/geocode")]
public class GeocodingController : ControllerBase
{
    private readonly GeocodingService _geocodingService;

    public GeocodingController(GeocodingService geocodingService)
    {
        _geocodingService = geocodingService;
    }

[HttpGet]
public async Task<IActionResult> GetCoordinates([FromQuery] string street, [FromQuery] string city, [FromQuery] string state, [FromQuery] string zip)
{
    try
    {
        Console.WriteLine($"Received request with: {street}, {city}, {state}, {zip}");
        var (lat, lon) = await _geocodingService.GetCoordinatesAsync(street, city, state, zip);

        if (lat == 0 && lon == 0)
            return NotFound("Could not geocode address.");

        return Ok(new { latitude = lat, longitude = lon });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GeocodingController: {ex.Message}");
        return StatusCode(500, "Internal Server Error.");
    }
}

}
