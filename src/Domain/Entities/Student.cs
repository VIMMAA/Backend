namespace Domain.Entities;

public class Student : User
{
    public required Guid GroupId { get; set; }
}