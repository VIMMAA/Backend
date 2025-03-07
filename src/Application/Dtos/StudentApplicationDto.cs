using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos;

public class StudentApplicationDto
{
    public required Guid Id { get; set; }
    public required StudentDto Student { get; set; }
    public required IEnumerable<LessonDto> Lessons { get; set; }
    public required DateTime ApplicationDate { get; set; }
    public required ApplicationStatus Status { get; set; }
    public required IEnumerable<AttachedFileResponseDto> AttachedFiles { get; set; }
}