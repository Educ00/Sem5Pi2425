using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Sem5Pi2425.Domain.OperationTypeAggr;
using OperationType = Microsoft.AspNetCore.JsonPatch.Operations.OperationType;

public class OperationTypeDto
{
    public OperationTypeId Id { get; set; }
    public DateTime Duration { get; set; }
    public Name Name { get; set; }
    public Description Description { get; set; }
    public List<string> NeededSpecializations { get; set; }
    
    public OperationTypeDto(Sem5Pi2425.Domain.OperationTypeAggr.OperationType operationType): this(operationType.Id, operationType.Duration, operationType.Name, operationType.Description, operationType.NeededSpecializations)
    {
    }


    public OperationTypeDto(OperationTypeId id, DateTime duration, Name name, Description description, List<string> neededSpecializations)
    {
        Id = id;
        Duration = duration;
        Name = name;
        Description = description;
        NeededSpecializations = neededSpecializations;
    }
}