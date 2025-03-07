using Application.Abstractions;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(
        IApplicationService applicationService,
        ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(StudentApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateApplicationAsync(
        [FromBody] CreateApplicationDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _applicationService.CreateApplicationAsync(request);
            return CreatedAtAction(
                nameof(GetApplicationByIdAsync),
                new { applicationId = result.Id },
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{applicationId}")]
    [ProducesResponseType(typeof(StudentApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateApplicationAsync(
        [FromRoute] Guid applicationId,
        [FromBody] UpdateApplicationDto request)
    {
        if (applicationId != request.ApplicationId)
        {
            return BadRequest("Application ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _applicationService.UpdateApplicationAsync(request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Application not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{applicationId}")]
    [ProducesResponseType(typeof(StudentApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteApplicationAsync(
        [FromRoute] Guid applicationId)
    {
        try
        {
            var result = await _applicationService.DeleteApplicationAsync(applicationId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Application not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting application");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllApplicationsAsync()
    {
        var result = await _applicationService.GetAllApplicationsAsync();
        return Ok(result);
    }

    [HttpGet("{applicationId}")]
    [ProducesResponseType(typeof(StudentApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplicationByIdAsync(
        [FromRoute] Guid applicationId)
    {
        try
        {
            var result = await _applicationService.GetByIdAsync(applicationId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Application not found");
            return NotFound();
        }
    }

    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(IEnumerable<StudentApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentApplicationsAsync(
        [FromRoute] Guid studentId)
    {
        var result = await _applicationService.GetStudentApplicationsAsync(studentId);
        return Ok(result);
    }

    [HttpPatch("{applicationId}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveApplicationAsync(
        [FromRoute] Guid applicationId)
    {
        try
        {
            await _applicationService.ApproveApplicationAsync(applicationId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Application not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving application");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPatch("{applicationId}/decline")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeclineApplicationAsync(
        [FromRoute] Guid applicationId)
    {
        try
        {
            await _applicationService.DeclineApplicationAsync(applicationId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Application not found");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error declining application");
            return StatusCode(500, "Internal server error");
        }
    }
}