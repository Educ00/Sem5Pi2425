using System;
using System.Linq;
using Sem5Pi2425.Domain.Shared;
using System.Text.RegularExpressions;

namespace Sem5Pi2425.Domain.SystemUser;

public class Password : IValueObject {
    private const int MinLength = 10;
    private const int MinUpperCase = 1;
    private const int MinDigits = 1;
    private const int MinSpecialChars = 1;

    public string Value { get; private set; }

    protected Password() { }

    public Password(string pw) {
        if (IsPasswordStrong(pw)) {
            this.Value = pw;
        }
        else {
            throw new BusinessRuleValidationException(
                "Password must have at least " + MinLength + " characters, " + MinUpperCase + " uppercase, " +
                MinDigits + " digit and " + MinSpecialChars + " special character.");
        }
    }

    public static bool IsPasswordStrong(string pw) {
        if (string.IsNullOrEmpty(pw) || pw.Length < MinLength)
            return false;

        var hasUpperCase = new Regex(@"[A-Z]");
        var hasDigit = new Regex(@"[0-9]");
        var hasSpecialChar = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

        
        return hasUpperCase.Matches(pw).Count >= MinUpperCase 
               && hasDigit.Matches(pw).Count >= MinDigits 
               && hasSpecialChar.Matches(pw).Count >= MinSpecialChars;
    }
}