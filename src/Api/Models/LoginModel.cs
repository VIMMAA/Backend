using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class LoginModel
{
    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6)]
    public required string Password { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }


}