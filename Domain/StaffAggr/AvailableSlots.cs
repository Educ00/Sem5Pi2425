﻿using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.StaffAggr;

public class AvailableSlots : IValueObject{
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    
    protected AvailableSlots() { }

    public AvailableSlots(DateTime start, DateTime end) {
        this.Start = start;
        this.End = end;
    }
}