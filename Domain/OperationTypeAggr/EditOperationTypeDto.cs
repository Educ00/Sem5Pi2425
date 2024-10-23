using System.Collections.Generic;
#nullable enable
namespace Sem5Pi2425.Domain.OperationTypeAggr;

public class EditOperationTypeDto
{
    public string? Id { get; set; }
    public string? Duration { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? NeededSpecializations { get; set; }
    public string? Active { get; set; }
        
        
    public EditOperationTypeDto(string id, string duration, string name, string description,
        string neededSpecializations, string active) {
        Id = id;
        Duration = duration;
        Name = name;
        Description = description;
        NeededSpecializations = neededSpecializations;
        Active = active;
    }
}