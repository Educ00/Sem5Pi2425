#nullable enable
using System;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.LogAggr;

public class Log : Entity<LogId> {
    public Type Type { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? Value { get; private set; }
    
    protected Log () { }

    public Log(Type type, string? text) {
        if (!IsValidText(text)) {
            throw new BusinessRuleValidationException("Log text should contain text!");
        }
        this.Id = LogId.NewLogId();
        this.Type = type;
        this.Value = text;
        Timestamp = DateTime.UtcNow;
    }

    public Log(Type type, string? text, UserId relatedUserId) {
        if (!IsValidText(text)) {
            throw new BusinessRuleValidationException("Log text should contain text!");
        }
        this.Id = LogId.NewLogId();
        this.Type = type;
        this.Value = "User: " + relatedUserId.Value + "\nLog: " + text;
        this.Timestamp = DateTime.UtcNow;
    }

    private bool IsValidText(string? text) {
        return !string.IsNullOrWhiteSpace(text);
    }
}