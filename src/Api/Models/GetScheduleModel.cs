namespace Api.Models;

public class GetScheduleModel
{
    public required DateTime DateFrom { get; set; }
    public required DateTime DateTo { get; set; }
}