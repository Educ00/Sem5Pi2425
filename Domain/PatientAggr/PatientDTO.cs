using System;
using System.Collections.Generic;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.AppointmentAggr;

namespace Sem5Pi2425.Domain.PatientAggr {
    public class PatientDto {
        public UserDto User { get; set; }
        public EmergencyContact EmergencyContact { get; set; }
        public List<MedicalCondition> MedicalConditions { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
        public List<Appointment> AppointmentsHistory { get; set; }
        
        
        public PatientDto(UserDto user, EmergencyContact emergencyContact, List<MedicalCondition> medicalConditions, DateOnly birthDate, Gender gender, List<Appointment> appointmentsHistory) {
            this.User = user;
            this.EmergencyContact = emergencyContact;
            this.MedicalConditions = medicalConditions;
            this.BirthDate = birthDate;
            this.Gender = gender;
            this.AppointmentsHistory = appointmentsHistory;
        }
    }
}