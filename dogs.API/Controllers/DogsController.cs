using dogs.Models;
using dogs.Services;
using Microsoft.AspNetCore.Mvc;

namespace dogs.Controllers;

[ApiController]
[Route("")]
public class DogsController : ControllerBase
{
    private readonly DogService _dogService;

    public DogsController(DogService dogService)
    {
        _dogService = dogService;
    }

    // GET /ping
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Dogshouseservice.Version1.0.1");
    }

    // GET /dogs
    [HttpGet("dogs")]
    public async Task<IActionResult> GetDogs([FromQuery] DogsQueryParams queryParams)
    {
        try
        {
            // Validate parameters
            if (!string.IsNullOrEmpty(queryParams.Attribute))
            {
                var validAttributes = new[] { "name", "color", "tail_length", "weight" };
                if (!validAttributes.Contains(queryParams.Attribute.ToLower()))
                {
                    return BadRequest("Invalid attribute. Use: name, color, tail_length, or weight");
                }
            }

            if (!string.IsNullOrEmpty(queryParams.Order))
            {
                if (queryParams.Order.ToLower() != "asc" && queryParams.Order.ToLower() != "desc")
                {
                    return BadRequest("Invalid order. Use: asc or desc");
                }
            }

            if (queryParams.PageNumber < 1 || queryParams.PageSize < 1)
            {
                return BadRequest("Page number and page size must be positive");
            }

            var dogs = await _dogService.GetDogsAsync(queryParams);
            return Ok(dogs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    // POST /dog
    [HttpPost("dog")]
    public async Task<IActionResult> CreateDog([FromBody] CreateDogRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _dogService.DogNameExistsAsync(request.Name))
            {
                return BadRequest("Dog with this name already exists");
            }

            var dog = await _dogService.CreateDogAsync(request);

            return Ok(dog);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}