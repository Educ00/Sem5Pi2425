using System;
using System.Collections.Generic;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SurgeryRoomAggr;

namespace Sem5Pi2425.Domain.AppointmentAggr
{
    public class AppointmentDTO
    {
        public AppointmentId Id { get; set; }
        public Status Status { get; set; }
        public DateTime DateTime { get; set; }
        public RoomNumber RoomNumber { get; set; }
        public List<Staff> AssignedStaffList { get; set; }
        public OperationRequest OperationRequest { get; set; }
        
        public AppointmentDTO(Appointment appointment) : this(appointment.Id, appointment.Status, appointment.DateTime, appointment.RoomNumber, appointment.AssignedStaffList, appointment.OperationRequest) {}

        public AppointmentDTO(AppointmentId id, Status status, DateTime dateTime, RoomNumber roomNumber,
            List<Staff> assignedStaffList, OperationRequest operationRequest)
        {
            Id = id;
            Status = status;
            DateTime = dateTime;
            RoomNumber = roomNumber;
            AssignedStaffList = assignedStaffList;
            OperationRequest = operationRequest;
        }
        
    }
    
}