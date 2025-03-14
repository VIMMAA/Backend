using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities.Application;

public class ApplicationModel : Entity
{
    public required Guid StudentId { get; set; }
    public required DateTime SubmissionDate { get; set; }
    public required ApplicationResult Status { get; set; }
    public virtual required ICollection<Lesson> Lessons { get; set; }
    public virtual required ICollection<AttachedFile> AttachedFiles { get; set; }
    public string? Comment { get; set; }
}
