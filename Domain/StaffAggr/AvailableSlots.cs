using System;
using System.Collections.Generic;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.StaffAggr
{
    public class AvailableSlots : IValueObject
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public List<string> Value { get; set; }

        protected AvailableSlots() { }

        public AvailableSlots(DateTime start, DateTime end)
        {
            this.Start = start;
            this.End = end;
        }

        public AvailableSlots(List<string> dtoAvailableSlotsList)
        {
            this.Value = dtoAvailableSlotsList;
        }

        public AvailableSlots(string slot)
        {
            if (string.IsNullOrWhiteSpace(slot))
            {
                throw new ArgumentException("Slot cannot be null or empty", nameof(slot));
            }

            // Assuming the slot string contains start and end times separated by a hyphen
            var times = slot.Split('-');
            if (times.Length != 2 || !DateTime.TryParse(times[0], out var start) || !DateTime.TryParse(times[1], out var end))
            {
                throw new ArgumentException("Slot must contain valid start and end times separated by a hyphen", nameof(slot));
            }

            this.Start = start;
            this.End = end;
        }
    }
}