
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


namespace MyApi.MapControllers 
{


[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly ApplicationContext _context;

    private readonly ITokenRevocationService _tokenRevocationService;

    public UserController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
    {
        _context = context;
      //  _configuration = configuration;
        _tokenRevocationService = tokenRevocationService;
    }


    /// <summary>
    /// Register new user
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("register")]
        [AllowAnonymous]
    public async Task<IActionResult> RegisterUser ([FromBody] UserRegisterModel userRegisterModel) 
    {


        if (!ModelState.IsValid) 
        {
            return BadRequest(new { status = "error", message = "Invalid arguments" });
        }

        User user = new()
        {
            Role = Role.Student,
            Id = Guid.NewGuid(),
            FirstName = userRegisterModel.FirstName,
            MiddleName = userRegisterModel.MiddleName,
            LastName = userRegisterModel.LastName,
            Birthday = userRegisterModel.Birthday,
            Email = userRegisterModel.Email,
            Password = userRegisterModel.Password,
        }  ;

      
        bool isEmailExist = _context.Users.Any(d => d.Email == user.Email);

        if (isEmailExist) 
        {
            return BadRequest(new { status = "error", message = "Email is already exists" });
        }

        try
        {
            await _context.Users.AddAsync(user);

            int r = await _context.SaveChangesAsync();

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = "G7@!f4#Zq8&lN9^kP2*eR1$hW3%tX6@zB5"; 

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT_KEY не установлен в переменных окружения.");
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                    new Claim(ClaimTypes.NameIdentifier, user.Email),

                }),
                Expires = DateTime.UtcNow.AddHours(1), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString }); 
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error registering user: {e}");
            return StatusCode(500, new  { Status = "error", Message = e.Message }); 
        }
    }   

       [ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
  [HttpPost("login")]
    [AllowAnonymous]

    public async Task<IActionResult> LoginDoctor([FromBody] LoginModel doctorLogin)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(new { status = "error", message = "Invalid arguments" });
        }


        var user = await _context.Users.FirstOrDefaultAsync(d => d.Email == doctorLogin.Email);
        

        if (user == null  )
        {
            return Unauthorized(new { status = "error", message = "Invalid email or password" });
        }

        var jwtKey = "G7@!f4#Zq8&lN9^kP2*eR1$hW3%tX6@zB5"; 

        if (string.IsNullOrEmpty(jwtKey))
        {
            return StatusCode(500, new { status = "error", message = "JWT key not found" });
        }


        var key = Encoding.ASCII.GetBytes(jwtKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Email),

            }),
            Expires = DateTime.UtcNow.AddMinutes(30), 
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var Token = tokenHandler.CreateToken(accessTokenDescriptor);
        var TokenString = tokenHandler.WriteToken(Token);
    try 
    {
        return Ok(new
        {
            status = "success",
            message = "User logged in successfully",
            role = user.Role,
            Token = TokenString,
        });
    }
    catch(Exception e)
     {
        Console.Error.WriteLine($"Error loggin doctor: {e}");

        return StatusCode(500); 
    }
    }

     [ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("logout")]
    [Authorize]
    public IActionResult LogoutDoctor()
    {
    
    var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    
    if (string.IsNullOrEmpty(token))
    {
        return BadRequest(new { message = "Token is required" });
    }

    if (!_tokenRevocationService.RevokeToken(token))
    {
        return BadRequest(new { message = "Token has already been revoked" });
    }

    return Ok(new { message = "Logout successful" });

    }


}


}