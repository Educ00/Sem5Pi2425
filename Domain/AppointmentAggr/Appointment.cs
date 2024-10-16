using System;
using System.Collections.Generic;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SurgeryRoomAggr;

namespace Sem5Pi2425.Domain.AppointmentAggr;

public class Appointment : Entity<AppointmentId>, IAggregateRoot {
    public Status Status { get; private set; }
    public DateTime DateTime { get; private set; }
    public RoomNumber RoomNumber { get; private set; }
    public List<Staff> AssignedStaffList { get; private set; }
    public OperationRequest OperationRequest { get; private set; }
    
    protected Appointment() {}

    public Appointment(Status status, DateTime dateTime, RoomNumber roomNumber, List<Staff> assignedStaffList, OperationRequest operationRequest) {
        this.Status = status;
        this.DateTime = dateTime;
        this.RoomNumber = roomNumber;
        this.AssignedStaffList = assignedStaffList;
        this.OperationRequest = operationRequest;
    }
}