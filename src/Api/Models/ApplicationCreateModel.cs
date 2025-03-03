using Domain.Abstractions;
using Domain.Enums;
using Microsoft.VisualBasic;
namespace Domain.Entities.Application;

public class ApplicationCreateModel 
{
    public required List <string> Lessons { get; set; }
    public required List <AttachedFileModel> Files {get ; set;}
}
