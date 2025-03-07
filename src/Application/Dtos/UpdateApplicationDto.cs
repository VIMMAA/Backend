namespace Application.Dtos;

public class UpdateApplicationDto
{
    public required Guid ApplicationId { get; set; }
    public required IEnumerable<AttachedFileRequestDto> AttachedFiles { get; set; }
    public required IEnumerable<Guid> Lessons { get; set; }
}