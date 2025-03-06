namespace Domain.Entities.Application;

public class ApplicationEditModel 
{
    public required List <string> Lessons { get; set; }
    public required List <AttachedFileModel> Files {get ; set;}
}