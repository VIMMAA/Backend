using Domain.Abstractions;
using Domain.Enums;
namespace Domain.Entities.Application;

public class ApplicationShortModel : Entity
{
    public required ApplicationResult Status { get; set; }
    
    public  required DateTime SubmissionDate { get; set; }

}
