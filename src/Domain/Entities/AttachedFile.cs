using Domain.Abstractions;


public class AttachedFile : Entity
{
    public required string Name { get; set; }
    public required string  FilePath { get; set; }

}