using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Domain.Entities;

public class AttachedFileModel 
{
    [Required]
    public required string Name { get; set; }
        
    [Required]
    public required string Data { get; set; }

}