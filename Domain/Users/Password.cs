using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.Users;

public class Password : IValueObject {

    private const int MinNumCount = 8;
    private const int MinSpecialCount = 1;
    private const int MinUpperCount = 1;
    
    public string Value { get; private set; }

    protected Password() {}
    
    public Password(string pw) {
        if (CheckPassword(pw)) {
            this.Value = pw;
        }
        
    }

    private bool CheckPassword(string pw) {
        var numCount = 0;
        var specialCount = 0;
        var upperCount = 0;
        foreach (var c in pw) {
            if (char.IsNumber(c)) {
                numCount++;
            } else if (!char.IsLetterOrDigit(c)) {
                specialCount++;
            } else if (char.IsUpper(c)) {
                upperCount++;
            }
        }

        if (numCount < MinNumCount || specialCount < MinSpecialCount || upperCount < MinUpperCount) {
            throw new BusinessRuleValidationException("Password must have at least " + MinNumCount + " numbers (" +
                                                      numCount + "), " +
                                                      MinSpecialCount + " special chars (" + specialCount + ") and " +
                                                      MinUpperCount + " upper case chars (" + upperCount + "): ->" +
                                                      pw);
        }

        return true;
    }
}