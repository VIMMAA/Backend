
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

    private readonly ApplicationContext _context;

    public ApplicationController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
    {
        _context = context;
        _tokenRevocationService = tokenRevocationService;
    }

    [DisableRequestSizeLimit]
    [HttpPost]
    public async Task<IActionResult> SubmitApplication([FromBody] ApplicationCreateModel applicationDto, Guid? id = null, DateTime? submissionDate = null)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
        }

        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (_tokenRevocationService.IsTokenRevoked(token))
        {
            return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
        }

        if (applicationDto.Files == null || applicationDto.Files.Count == 0)
        {
            return BadRequest("Необходимо прикрепить 1 файл или более");
        }

        if (applicationDto.Lessons == null || applicationDto.Lessons.Count == 0)
        {
            return BadRequest("Добавьте уроки, пожалуйста");
        }

        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var application = new ApplicationModel
        {
            AttachedFiles = applicationDto.Files.ConvertAll(m => new AttachedFile
            {
                Name = m.Name,
                Id = Guid.NewGuid(),
                Data = m.Data
            }),
            StudentId = userId,
            Id = id ?? Guid.NewGuid(),
            Status = ApplicationStatus.NotDefined,
            SubmissionDate = submissionDate ?? DateTime.UtcNow,
            Comment = null,
            Lessons = []
        };

        foreach (var lesson in _context.Lessons.Where(lesson => applicationDto.Lessons.Contains(lesson.Id.ToString())))
        {
            application.Lessons.Add(lesson);
        }

        await _context.Applications.AddAsync(application);
        await _context.SaveChangesAsync();

        return Ok(new { status = "success", message = "application has created", application = application });
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    [Authorize]
    [DisableRequestSizeLimit]
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

        var application = await _context.Applications
            .Include(a => a.AttachedFiles)
            .Include(a => a.Lessons)
            .FirstOrDefaultAsync(a => a.Id == id);

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
            return StatusCode(500, new { Status = "error", Message = "JWT key not found" });
        }
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("applicationList/{StudentId}")]
    [Authorize]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<List<ApplicationShortModel>>> GetShortApplication(Guid StudentId)
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
            .Where(a => a.StudentId == StudentId)
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
            return StatusCode(500, new { Status = "error", Message = e.Message });
        }
    }

    [HttpPut("{id}")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UpdateApplication(Guid id, [FromBody] ApplicationEditModel applicationEditModel)
    {
        var application = await _context.Applications.FindAsync(id);

        await DeleteApplicationAsync(id);
        await SubmitApplication(new ApplicationCreateModel()
        {
            Lessons = applicationEditModel.Lessons,
            Files = applicationEditModel.Files,
        }, id, application!.SubmissionDate);

        return Ok(new { status = "success", message = "Application has updated" });

        //var application = await _context.Applications
        //    .Include(a => a.AttachedFiles)
        //    .Include(a => a.Lessons)
        //    .FirstOrDefaultAsync(a => a.Id == id);

        //if (application == null)
        //{
        //    return NotFound(new { status = "error", message = "Заявка не найдена" });
        //}

        //application.Lessons.Clear();
        //foreach (var lessonId in applicationEditModel.Lessons)
        //{
        //    var lesson = await _context.Lessons.FindAsync(Guid.Parse(lessonId));
        //    if (lesson != null)
        //    {
        //        application.Lessons.Add(lesson);
        //    }
        //}

        //application.SubmissionDate = DateTime.UtcNow;
        //application.Status = ApplicationStatus.NotDefined;
        //var sortedApplications = await _context.Applications
        //    .OrderBy(a => a.SubmissionDate) 
        //    .ToListAsync();

        //await _context.SaveChangesAsync();

        //return Ok(new { status = "success", message = "Application has updated" });
    }

    [HttpPost("approve/{id}")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> ApproveApplicationAsync(Guid id)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
        {
            return NotFound(new { status = "error", message = "Заявка не найдена" });
        }

        application.Status = ApplicationStatus.Approved;
        await _context.SaveChangesAsync();
        return Ok(new { status = "success", message = "Заявка одобрена" });
    }
    [DisableRequestSizeLimit]
    [HttpPost("decline/{id}")]
    public async Task<IActionResult> DeclineApplicationAsync(Guid id)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
        {
            return NotFound(new { status = "error", message = "Заявка не найдена" });
        }

        application.Status = ApplicationStatus.Declined;
        await _context.SaveChangesAsync();
        return Ok(new { status = "success", message = "Заявка отклонена" });
    }

    [HttpDelete("{id}")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> DeleteApplicationAsync(Guid id)
{
    try
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
        {
            return NotFound(new { status = "error", message = "Заявка не найдена" });
        }

        var files = _context.Files
            .Where(f => EF.Property<Guid>(f, "ApplicationModelId") == id);

        var lessons = _context.Lessons
            .Where(l => EF.Property<Guid?>(l, "ApplicationModelId") == id);

        foreach (var lesson in lessons)
        {
            _context.Entry(lesson).Property("ApplicationModelId").CurrentValue = null;
        }

        _context.Files.RemoveRange(files);
        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return Ok(new { status = "success", message = "Заявка удалена" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { status = "error", message = "Ошибка сервера" });
    }
}

    [DisableRequestSizeLimit]
    [HttpGet("applicationList")]
    public async Task<IActionResult> GetAllApplicationsAsync()
    {
        var applications = await _context.Applications
            .Include(a => a.AttachedFiles)
            .Include(a => a.Lessons)
            .ToListAsync();
        return Ok(new { applications });
    }
}