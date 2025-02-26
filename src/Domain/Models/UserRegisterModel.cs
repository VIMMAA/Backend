using Domain.Abstractions;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Domain.Models
{
    public abstract class UserRegisterModel 
    {
        [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Name { get; set; }

    [Required]
    [StringLength(int.MaxValue, MinimumLength = 6)]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public DateTime Birthday { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Phone]
    public string Phone { get; set; }

    [Required]
    public Guid Speciality{ get; set; }

    }
}