using System.Globalization;

namespace Sem5Pi2425.Domain.LogAggr;

public class LogDto {
    public string Id { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public string UserId { get; set; }
    public string Timestamp { get; set; }

    public LogDto(string id, string type, string value, string userId,string timestamp) {
        this.Id = id;
        this.Type = type;
        this.Value = value;
        this.UserId = userId;
        this.Timestamp = timestamp;
    }
}