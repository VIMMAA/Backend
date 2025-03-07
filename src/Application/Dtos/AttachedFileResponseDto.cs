namespace Application.Dtos;

public class AttachedFileResponseDto
{
    public required Guid FileId { get; set; }
    public required string Name { get; set; }
}