using Domain.Abstractions;

namespace Domain.Entities;

public class Lesson : Entity
{
    public required string Name { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
}