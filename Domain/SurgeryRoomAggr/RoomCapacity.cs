using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SurgeryRoomAggr;

public class RoomCapacity : IValueObject{
    public int Value { get; private set; }
    
    protected RoomCapacity() { }

    public RoomCapacity(int value) {
        if (value <= 0) {
            throw new BusinessRuleValidationException("Capacity must be > 0.");
        }
        this.Value = value;
    }
}