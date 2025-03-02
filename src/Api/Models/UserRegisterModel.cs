using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class UserRegisterModel
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string FirstName { get; set; }
    
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string MiddleName { get; set; }

     [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string LastName { get; set; }

    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6)]
    public required string Password { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public required DateOnly Birthday { get; set; }

}