using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SurgeryRoomAggr;

public class Equipment : IValueObject {
    public string Name { get; private set; }
    
    protected Equipment() { }

    public Equipment(string name) {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new BusinessRuleValidationException("Invalid name ->" + name);
        }
        this.Name = name;
    }
}