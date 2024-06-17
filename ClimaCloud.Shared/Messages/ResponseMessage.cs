namespace ClimaCloud.Shared.Messages
{
    public class ResponseMessage
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ResponseMessage<T> : ResponseMessage
    {
        public T? Data { get; set; }
    }
}
