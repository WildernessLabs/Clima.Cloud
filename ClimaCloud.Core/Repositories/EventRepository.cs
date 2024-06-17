using Azure;
using Azure.Data.Tables;
using ClimaCloud.Core.Common;
using ClimaCloud.Core.Entities;
using ClimaCloud.Core.Interfaces;

namespace ClimaCloud.Core.Repositories
{
    public class EventRepository : RepositoryBase, IEventRepository
    {
        public EventRepository(TableClient tableClient) : base(tableClient) { }

        public AsyncPageable<Event> GetAllEventsAsync()
        {
            return GetFullCloudListAsync<Event>(
            $"PartitionKey gt 'EVENT_' and PartitionKey lt 'EVENU'") ??
            AsyncPageable<Event>.FromPages(Enumerable.Empty<Page<Event>>());
        }

        public async Task<Event?> GetEventByDeviceIdAsync(string orgId, string deviceId)
        {
            return await GetAsync<Event>(Keys.DeviceEventPartitionKey(orgId), Keys.DeviceEventRowKey(deviceId));
        }

        public async Task<Event?> SaveEventAsync(Event @event)
        {
            @event.PartitionKey = Keys.DeviceEventPartitionKey(@event.OrgId);
            @event.RowKey = Keys.DeviceEventRowKey(@event.DeviceId);

            return await UpsertAsync<Event>(@event);
        }
    }
}