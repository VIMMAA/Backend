using Api.Context;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class InvitationLinkController : ControllerBase
{
    private readonly ApplicationContext _context;

    public InvitationLinkController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GenerateInvitationLinkAsync([FromQuery] Role role)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
        }

        var link = new InvitationLink
        {
            Id = Guid.NewGuid(),
            Role = role,
        };
        _context.InvitationLinks.Add(link);
        await _context.SaveChangesAsync();
        return Ok(new { generatedLink = link.Id });
    }
}