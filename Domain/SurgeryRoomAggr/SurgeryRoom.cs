using System;
using System.Collections.Generic;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SurgeryRoomAggr;

public class SurgeryRoom : Entity<RoomNumber>, IAggregateRoot {
    public RoomCapacity RoomCapacity { get; private set; }
    public List<Equipment> AssignedEquipment { get; private set; }
    public Status Status { get; private set; }
    public Type Type { get; private set; }
    
    protected SurgeryRoom() {}

    public SurgeryRoom(RoomNumber roomNumber, RoomCapacity roomCapacity, List<Equipment> assignedEquipment, Status status, Type type) {
        this.Id = roomNumber;
        this.RoomCapacity = roomCapacity;
        this.AssignedEquipment = assignedEquipment;
        this.Status = status;
        this.Type = type;
    }

    public void AddEquipment(Equipment equipment) {
        if (!this.AssignedEquipment.Contains(equipment)) {
            this.AssignedEquipment.Add(equipment);
        }
    }

    public void RemoveEquipment(Equipment equipment) {
        this.AssignedEquipment.Remove(equipment);
    }
    
}