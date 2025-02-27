using Domain.Abstractions;

namespace Domain.Entities;

public class AttachedFile : Entity
{
    public required string Name { get; set; }
}