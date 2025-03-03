using Domain.Abstractions;
using Domain.Enums;
using Microsoft.VisualBasic;
namespace Domain.Entities.Application;

public class ApplicationModel : Entity
{
    public required List <Lesson> Lessones {get; set;}

    public required ApplicationResult Status { get; set; }

    public required Guid StudentId { get; set; }
    public  required DateTime SubmissionDate { get; set; }

    public required List <AttachedFile> AttachedFiles {get; set;}
}
