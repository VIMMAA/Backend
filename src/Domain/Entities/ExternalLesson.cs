namespace Api.Models;

public class ExternalLesson
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required int Starts { get; set; }
    public required int Ends { get; set; }
}