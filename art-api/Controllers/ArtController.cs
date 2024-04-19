using ArtsieApi.Models;
using ArtsieApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace artStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtController : ControllerBase
{
    private readonly ArtsieService _artService;

    public ArtController(ArtsieService artService) =>
        _artService = artService;

    [HttpGet]
    public async Task<List<Art>> Get() =>
        await _artService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Art>> Get(string id)
    {
        var art = await _artService.GetAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        return art;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Art newart)
    {
        await _artService.CreateAsync(newart);

        return CreatedAtAction(nameof(Get), new { id = newart.Id }, newart);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Art updatedart)
    {
        var art = await _artService.GetAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        updatedart.Id = art.Id;

        await _artService.UpdateAsync(id, updatedart);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var art = await _artService.GetAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        await _artService.RemoveAsync(id);

        return NoContent();
    }
}