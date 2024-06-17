using ClimaCloud.Shared.Messages;

namespace ClimaCloud.Core.Interfaces
{
    public interface IEventService
    {
        Task<ResponseMessage<List<ClientEventMessage>>> GetAllEventsAsync();
        Task<ResponseMessage<EventMessage?>> SaveEventAsync(EventMessage deviceEventMessage);
    }
}
