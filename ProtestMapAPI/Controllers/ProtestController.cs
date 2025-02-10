using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtestMapAPI.Data;
using ProtestMapAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ProtestMapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProtestController> _logger;
        private readonly GeocodingService _geocodingService;

        public ProtestController(ApplicationDbContext context, ILogger<ProtestController> logger, GeocodingService geocodingService)
        {
            _context = context;
            _logger = logger;
            _geocodingService = geocodingService;
        }

        // GET: api/protests - Get all protests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Protest>>> GetProtests()
        {
            return await _context.Protests.ToListAsync();
        }

        // GET: api/protests/{id} - Get a specific protest by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Protest>> GetProtest(int id)
        {
            var protest = await _context.Protests.FindAsync(id);

            if (protest == null)
            {
                return NotFound();
            }

            return protest;
        }

        // GET: api/protests/test-auth - Test authentication status
        [HttpGet("test-auth")]
        [Authorize]
        public ActionResult TestAuth()
        {
            return Ok(new
            {
                message = "Authorization successful",
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        // POST: api/protests - Create a new protest (protected)
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Protest>> PostProtest([FromBody] Protest protest)
        {
            try
            {
                _logger.LogInformation("User authenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated ?? false);
                _logger.LogInformation("User claims: {Claims}", string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}")));

                var userName = User.FindFirstValue("name"); // Get the 'name' (email) claim from the token
                _logger.LogInformation("User name from claims: {UserName}", userName);

                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogWarning("User name not found in token");
                    return Unauthorized("User name not found in token");
                }

                // Retrieve the user by their email (which is stored in the "name" claim)
                var user = await _context.Users
                                          .FirstOrDefaultAsync(u => u.Email == userName);

                if (user == null)
                {
                    _logger.LogWarning("User not found with email: {UserName}", userName);
                    return Unauthorized("User not found");
                }

                // Retrieve the coordinates from the geocoding service
                var coordinates = await _geocodingService.GetCoordinatesAsync(protest.Street, protest.City, protest.State, protest.ZipCode);

                if (coordinates != (0, 0))
                {
                    protest.Latitude = coordinates.Item1;
                    protest.Longitude = coordinates.Item2;
                }
                else
                {
                    _logger.LogWarning("Geocoding failed for address: {Address}", protest.Street);
                    return BadRequest("Could not retrieve coordinates for the provided address");
                }

                // Set the CreatedByUserId to the actual user Id
                protest.CreatedByUserId = user.Id;

                if (ModelState.IsValid)
                {
                    _context.Protests.Add(protest);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(GetProtest), new { id = protest.Id }, protest);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating protest");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/protests/{id} - Update an existing protest (only by creator)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProtest(int id, Protest protest)
        {
            if (id != protest.Id)
            {
                return BadRequest();
            }

            var existingProtest = await _context.Protests.FindAsync(id);
            if (existingProtest == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (existingProtest.CreatedByUserId != userId)
            {
                return Forbid(); // Only the creator can update
            }

            _context.Entry(existingProtest).CurrentValues.SetValues(protest);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProtestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/protests/{id} - Delete a protest (only by creator)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProtest(int id)
        {
            var protest = await _context.Protests.FindAsync(id);
            if (protest == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (protest.CreatedByUserId != userId)
            {
                return Forbid(); // Only the creator can delete
            }

            _context.Protests.Remove(protest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProtestExists(int id)
        {
            return _context.Protests.Any(e => e.Id == id);
        }
    }
}
