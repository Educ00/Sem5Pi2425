﻿using System;
using System.Collections.Generic;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.PatientAggr;

public class Patient : Entity<UserId> {
    public User User { get; private set; }
    public EmergencyContact EmergencyContact { get; private set; }
    public List<MedicalCondition> MedicalConditions { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public List<Appointment> AppointmentsHistory { get; private set; }
    public bool MarkedForDeletion { get; private set; }
    
    protected Patient(){}

    public Patient(User user, EmergencyContact emergencyContact, List<MedicalCondition> medicalConditions, DateOnly birthDate, Gender  gender, List<Appointment> appointmentsHistory) {
        if (!user.Role.Equals(Role.patient)) {
            throw new BusinessRuleValidationException("Invalid user!");
        }
        this.Id = user.Id;
        this.User = user;
        this.EmergencyContact = emergencyContact;
        this.MedicalConditions = medicalConditions;
        this.BirthDate = birthDate;
        this.Gender = gender;
        this.AppointmentsHistory = appointmentsHistory;
    }

    public void MarkForDeletion() {
        this.MarkedForDeletion = true;
    }
}