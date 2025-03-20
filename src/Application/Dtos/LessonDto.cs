namespace Application.Dtos;

public class LessonDto
{
    public required string Id { get; set; }
    public required DateTime DateFrom { get; set; }
    public required DateTime DateTo { get; set; }
    public required string Name { get; set; }
}