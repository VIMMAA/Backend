namespace Application.Dtos;

public class CreateApplicationDto
{
    public required IEnumerable<AttachedFileRequestDto> AttachedFiles { get; set; }
    public required IEnumerable<Guid> Lessons { get; set; }
}