namespace Application.Dtos;

public class StudentDto
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
}