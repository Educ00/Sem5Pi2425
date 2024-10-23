using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.OperationTypeAggr;

public class Description : IValueObject{
    public string Value { get; private set; }
    
    protected Description() { }

    public Description(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new BusinessRuleValidationException("Invalid description ->" + value);
        }
        Value = value;
    }
}