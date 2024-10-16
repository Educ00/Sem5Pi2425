using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.OperationTypeAggr;

public class Name : IValueObject{
    public string Value { get; private set; }
    
    protected Name() { }

    public Name(string value) {
        this.Value = value;
    }
}