using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Application : Entity
{
    public required DateTime SubmissionDate { get; set; }
    public required ApplicationResult Result { get; set; }
    public virtual required ICollection<Lesson> SelectedLessons { get; set; }
    public virtual required ICollection<AttachedFile> AttachedFiles { get; set; }
}