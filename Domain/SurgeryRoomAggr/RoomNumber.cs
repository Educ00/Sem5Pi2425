using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SurgeryRoomAggr;

public class RoomNumber : EntityId {
    public RoomNumber(int value) : base(value) { }

    protected override string createFromString(string text) {
        return text;
    }

    public override string AsString() {
        return (string)ObjValue;
    }

    public static RoomNumber NewRoomNumber(int number) {
        return new RoomNumber(number);
    }
}