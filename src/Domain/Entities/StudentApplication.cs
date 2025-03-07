using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class StudentApplication : Entity
{
    public required DateTime SubmissionDate { get; set; }
    public required ApplicationStatus Status { get; set; }
    public virtual required ICollection<Lesson> Lessons { get; set; }
    public virtual required ICollection<AttachedFile> AttachedFiles { get; set; }
}