
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using System.Security.Claims; 
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Api.Context;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Domain.Enums;
using Domain.Entities.Application;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly ITokenRevocationService _tokenRevocationService;

    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

    private readonly ApplicationContext _context;

    public ApplicationsController(ApplicationContext context)
    {
        _context = context;

        if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
    }

    [HttpPost]
    public async Task<IActionResult> SubmitApplication([FromBody] ApplicationCreateModel applicationDto)
    {

    var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

    if (_tokenRevocationService.IsTokenRevoked(token))
    {
        return Unauthorized(new { status = "error", message = "Unauthorized access" });
    }

    if (applicationDto.Files == null || applicationDto.Files.Count == 0)
    {
        return BadRequest("You need to attach 1 file or more");
    }

    if (applicationDto.Lessons == null || applicationDto.Lessons.Count == 0)
    {
        return BadRequest("Add lessones, pls");
    }

    var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
        return BadRequest(new { message = "Invalid token" });
    }

    var userId = Guid.Parse(userIdClaim.Value);



    var application = new ApplicationModel
    {
        AttachedFiles = new List<AttachedFile>(),
        StudentId = userId,
        Id = Guid.NewGuid(),
        Status = ApplicationResult.NotDefined,
        SubmissionDate = DateTime.UtcNow,
        Lessones = new List<Lesson>()
    };

    foreach (var file in applicationDto.Files)
    {
        var fileName = Path.GetFileName(file.Name);
        var filePath = Path.Combine(_storagePath, fileName);

        var base64Data = file.Data.Split(',')[1];
        var fileBytes = Convert.FromBase64String(base64Data);

        await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

        var attachedFile = new AttachedFile
        {
            Id = Guid.NewGuid(),
            Name = fileName,
            FilePath = filePath,
        };

        application.AttachedFiles.Add(attachedFile);
    }

    foreach (var lesson in _context.Lessones) 
    {
        bool isInclude = applicationDto.Lessons.Any(a => a == lesson.Id.ToString());

        if (isInclude) 
        {
            application.Lessones.Add(lesson);
        }

    }

_context.Applications.Add(application);
await _context.SaveChangesAsync();

return Ok(new { message = "Application has created" , Application = application });
       
}
}





