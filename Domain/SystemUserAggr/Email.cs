﻿using System;
using System.Text.RegularExpressions;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class Email : IValueObject{
        private const string EmailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

        public string Value {
            get;
            private set;
        }

        protected Email() {
        }

        public Email(string value) {
            if (!IsValidEmail(value)) {
                throw new ArgumentException("Invalid email format");
            }
            this.Value = value;
        }
        private bool IsValidEmail(string email) {
            if (string.IsNullOrWhiteSpace(email)) {
                throw new ArgumentException("Email cannot be null or blank ->" + email);
            }

            try {
                return Regex.IsMatch(email, EmailPattern);
            }
            catch(RegexMatchTimeoutException e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string email) => new Email(email);
    }
}