namespace ClimaCloud.Shared.Messages
{
    public class BaseEventMessage
    {
        public string? Description { get; set; }
        public Dictionary<string, object> Measurements { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string DeviceId { get; set; }
        public string OrgId { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
