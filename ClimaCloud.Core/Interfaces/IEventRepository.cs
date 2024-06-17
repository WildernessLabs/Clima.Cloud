using Azure;
using ClimaCloud.Core.Entities;

namespace ClimaCloud.Core.Interfaces
{
    public interface IEventRepository
    {
        AsyncPageable<Event> GetAllEventsAsync();
        Task<Event?> GetEventByDeviceIdAsync(string orgId, string deviceId);
        Task<Event?> SaveEventAsync(Event @event);
    }
}