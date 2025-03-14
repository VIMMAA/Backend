using Api.Context;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class AttachedFileController : ControllerBase
{
    private readonly ApplicationContext _context;

    public AttachedFileController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFileAsync(Guid id)
    {
        var file = await _context.Files.FindAsync(id);
        if (file == null)
        {
            return NotFound();
        }
        return Ok(new { name = file.Name, data = file.Data });
    }
}