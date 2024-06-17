namespace ClimaCloud.Shared.Messages
{
    public class ClientEventMessage : BaseEventMessage
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}