using Domain.Abstractions;
namespace Domain.Entities.Application;

public class AttachedFile : Entity
{
    public required string Name { get; set; }
    public required string Data { get; set; }
}