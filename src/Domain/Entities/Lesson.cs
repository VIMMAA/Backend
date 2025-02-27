using Domain.Abstractions;

namespace Domain.Entities;

public class Lesson : Entity
{
    public required string Name { get; set; }
    public required DateTimeOffset StartTime { get; set; }
    public required DateTimeOffset EndTime { get; set; }
}