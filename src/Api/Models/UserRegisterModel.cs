using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class UserRegisterModel
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string Name { get; set; }

    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6)]
    public required string Password { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public required DateTime Birthday { get; set; }

    [Phone]
    public required string Phone { get; set; }

    [Required]
    public required Guid Speciality { get; set; }
}