
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
public class ApplicationController : ControllerBase
{
    private readonly ITokenRevocationService _tokenRevocationService;

    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

    private readonly ApplicationContext _context;

    public ApplicationController(ApplicationContext context)
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
    if (!User.Identity.IsAuthenticated)
    {
        return Unauthorized(new { status = "error", message = "Unauthorized access" });
    }

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
        Description = null,
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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    [Authorize] 

    public async Task<ActionResult<ApplicationModel>> GetApplication(Guid id) 
    {
    if (!User.Identity.IsAuthenticated)
    {
        return Unauthorized(new { status = "error", message = "Unauthorized access" });
    }

    var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

    if (_tokenRevocationService.IsTokenRevoked(token))
    {
        return Unauthorized(new { status = "error", message = "Unauthorized access" });
    }

    var application = await _context.Applications.FirstOrDefaultAsync(d => d.Id == id); 
    if (application == null)
    {
        return BadRequest(new { message = "Invalid Id" });
    }
    try
    {
        return Ok(application);
    }
    catch
    {
        return StatusCode(500, new  { Status = "error", Message = "JWT key not found" });
    }
}


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]   
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("applicationList/{StudentId}")]
    [Authorize] 
    public async Task<ActionResult<List<ApplicationShortModel>>> GetShortApplication(  Guid StudentId ) 
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new { status = "error", message = "Invalid arguments" });
    }

    var patient = await _context.Users.FirstOrDefaultAsync(d => d.Id == StudentId);
    
    if (patient == null)
    {
        return BadRequest(new { message = "Invalid Id" });
    }

    var applications = await _context.Applications
        .Where( a => a.StudentId == StudentId)
        .ToListAsync();

   

    
    var applicationShortModels = applications.Select(i => new ApplicationShortModel
    {
        Id = i.Id,
        SubmissionDate = i.SubmissionDate,
        Status = i.Status
    }).ToList();

    try 
    {
        return Ok(applicationShortModels); 
    }
    catch (Exception e)
        {
            Console.Error.WriteLine($"Error registering doctor: {e}");
            return StatusCode(500, new  { Status = "error", Message = e.Message }); 
        }
}


}








