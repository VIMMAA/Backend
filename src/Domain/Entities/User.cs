using Domain.Abstractions;
using Domain.Enums;
using Microsoft.VisualBasic;
namespace Domain.Entities;

public class User : Entity
{
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly Birthday { get; set; }
    public required string Email { get; set; }
    public required Role Role { get; set; }

    public required string Password { get; set; }

    public void Update (string firstName, string middleName , string lastName) 
    {
       FirstName = firstName;
       MiddleName = middleName;
       LastName = lastName;

    }
}