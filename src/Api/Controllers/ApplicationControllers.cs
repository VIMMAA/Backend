
// using Microsoft.AspNetCore.Mvc;
// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.EntityFrameworkCore;
// using Api.Models;
// using System.Security.Claims; 
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using System.IdentityModel.Tokens.Jwt;
// using Api.Context;
// using Microsoft.AspNetCore.Authorization;
// using Domain.Entities;
// using Domain.Enums;
// using Domain.Entities.Application;

// [ApiController]
// [Route("api/[controller]")]
// public class ApplicationController : ControllerBase
// {
//     private readonly ITokenRevocationService _tokenRevocationService;

//     private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

//     private readonly ApplicationContext _context;

//     public ApplicationController(ApplicationContext context)
//     {
//         _context = context;

//         if (!Directory.Exists(_storagePath))
//             {
//                 Directory.CreateDirectory(_storagePath);
//             }
//     }

//     [HttpPost]
//    public async Task<IActionResult> SubmitApplication([FromBody] ApplicationCreateModel applicationDto)
// {
//     if (!User.Identity.IsAuthenticated)
//     {
//         return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
//     }

//     var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

//     if (_tokenRevocationService.IsTokenRevoked(token))
//     {
//         return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
//     }

//     if (applicationDto.Files == null || applicationDto.Files.Count == 0)
//     {
//         return BadRequest("Необходимо прикрепить 1 файл или более");
//     }

//     if (applicationDto.Lessons == null || applicationDto.Lessons.Count == 0)
//     {
//         return BadRequest("Добавьте уроки, пожалуйста");
//     }

//     var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

//     if (userIdClaim == null)
//     {
//         return BadRequest(new { message = "Недействительный токен" });
//     }

//     var userId = Guid.Parse(userIdClaim.Value);

//     var application = new ApplicationModel
//     {
//         AttachedFiles = new List<AttachedFile>(),
//         StudentId = userId,
//         Id = Guid.NewGuid(),
//         Status = ApplicationResult.NotDefined,
//         SubmissionDate = DateTime.UtcNow,
//         Description = null,
//         Lessones = new List<Lesson>()
//     };

//     foreach (var file in applicationDto.Files)
//     {
//         if (string.IsNullOrEmpty(file.Data) || !file.Data.StartsWith("data:"))
//         {
//             return BadRequest("unvalid files");
//         }

//         var fileName = Path.GetFileName(file.Name);
//         var filePath = Path.Combine(_storagePath, fileName);

//         var base64Data = file.Data.Split(',')[1];
//         var fileBytes = Convert.FromBase64String(base64Data);

//         await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

//         var attachedFile = new AttachedFile
//         {
//             Id = Guid.NewGuid(),
//             Name = fileName,
//             FilePath = filePath,
//         };

//         application.AttachedFiles.Add(attachedFile);
//     }

//     foreach (var lesson in _context.Lessones) 
//     {
//         if (applicationDto.Lessons.Contains(lesson.Id.ToString())) 
//         {
//             application.Lessones.Add(lesson);
//         }
//     }

//     await _context.Applications.AddAsync(application);
//      await _context.SaveChangesAsync();

//     return Ok(new { status = "success", message = "application has created", application = application  });
// }

//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//     [HttpGet("{id}")]
//     [Authorize] 

//     public async Task<ActionResult<ApplicationModel>> GetApplication(Guid id) 
//     {
//     if (!User.Identity.IsAuthenticated)
//     {
//         return Unauthorized(new { status = "error", message = "Unauthorized access" });
//     }

//     var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

//     if (_tokenRevocationService.IsTokenRevoked(token))
//     {
//         return Unauthorized(new { status = "error", message = "Unauthorized access" });
//     }

//     var application = await _context.Applications
//         .Include(a => a.AttachedFiles) 
//         .Include(a => a.Lessones) 
//         .FirstOrDefaultAsync(a => a.Id == id);    
    
//     if (application == null)
//     {
//         return BadRequest(new { message = "Invalid Id" });
//     }
//     try
//     {
//         return Ok(application);
//     }
//     catch
//     {
//         return StatusCode(500, new  { Status = "error", Message = "JWT key not found" });
//     }
// }


//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     [ProducesResponseType(StatusCodes.Status401Unauthorized)]   
//     [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//     [HttpGet("applicationList/{StudentId}")]
//     [Authorize] 
//     public async Task<ActionResult<List<ApplicationShortModel>>> GetShortApplication(  Guid StudentId ) 
//     {
//     if (!ModelState.IsValid)
//     {
//         return BadRequest(new { status = "error", message = "Invalid arguments" });
//     }

//     var patient = await _context.Users.FirstOrDefaultAsync(d => d.Id == StudentId);
    
//     if (patient == null)
//     {
//         return BadRequest(new { message = "Invalid Id" });
//     }

//     var applications = await _context.Applications
//         .Where( a => a.StudentId == StudentId)
//         .ToListAsync();

   

    
//     var applicationShortModels = applications.Select(i => new ApplicationShortModel
//     {
//         Id = i.Id,
//         SubmissionDate = i.SubmissionDate,
//         Status = i.Status
//     }).ToList();

//     try 
//     {
//         return Ok(applicationShortModels); 
//     }
//     catch (Exception e)
//     {
//         Console.Error.WriteLine($"Error registering doctor: {e}");
//         return StatusCode(500, new  { Status = "error", Message = e.Message }); 
//     }
// }

// [HttpPut("{id}")]
// public async Task<IActionResult> UpdateApplication(Guid id, [FromBody] ApplicationEditModel applicationEditModel)
// {
//     var application = await _context.Applications
//         .Include(a => a.AttachedFiles)
//         .Include(a => a.Lessones)
//         .FirstOrDefaultAsync(a => a.Id == id);

//     if (application == null)
//     {
//         return NotFound(new { status = "error", message = "Заявка не найдена" });
//     }

//     application.Lessones.Clear(); 
//     foreach (var lessonId in applicationEditModel.Lessons)
//     {
//         var lesson = await _context.Lessones.FindAsync(Guid.Parse(lessonId));
//         if (lesson != null)
//         {
//             application.Lessones.Add(lesson);
//         }
//     }

//     application.AttachedFiles.Clear(); 
//     foreach (var file in applicationEditModel.Files)
//     {
        
//         var fileName = Path.GetFileName(file.Name);
//         var filePath = Path.Combine(_storagePath, fileName);

//         var base64Data = file.Data.Split(',')[1];
//         var fileBytes = Convert.FromBase64String(base64Data);

//         await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

//         var attachedFile = new AttachedFile
//         {
//             Id = Guid.NewGuid(), 
//             Name = file.Name,
//             FilePath = filePath 
//         };

//         application.AttachedFiles.Add(attachedFile);
//     }

//     await _context.SaveChangesAsync();

//     return Ok(new { status = "success", message = "Application has been updated" });
// }

// }








