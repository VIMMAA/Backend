
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
public class UserListController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

    public UserListController(ApplicationContext context)
    {
        _context = context;

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers([FromQuery] Role? role = null) 
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { status = "error", message = "Invalid arguments" });
        }

        List<User> userList;

        try 
        {
            if (role.HasValue) 
            {
                userList = await _context.Users
                    .Where(a => a.Role == role.Value) 
                    .ToListAsync();
            }
            else 
            {
                userList = await _context.Users.ToListAsync(); 
            }

            return Ok(userList); 
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Status = "error", Message = e.Message }); 
        }
    }
}









