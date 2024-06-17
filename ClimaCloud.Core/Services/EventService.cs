using ClimaCloud.Core.Entities;
using ClimaCloud.Core.Interfaces;
using ClimaCloud.Shared.Messages;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ClimaCloud.Core.Services
{
    public class EventService(IEventRepository eventRepository, ILogger<EventService> logger) : IEventService
    {
        public async Task<ResponseMessage<List<ClientEventMessage>>> GetAllEventsAsync()
        {
            var response = new ResponseMessage<List<ClientEventMessage>>();

            try
            {
                var result = await eventRepository.GetAllEventsAsync().ToListAsync();

                response.Data = result.Select(t => new ClientEventMessage
                {
                    Latitude = t.Latitude,
                    Longitude = t.Longitude,
                    Description = t.Description,
                    Measurements = JsonSerializer.Deserialize<Dictionary<string, object>>(t.MeasurementsRaw) ?? new Dictionary<string, object>(),
                    DeviceId = t.DeviceId,
                    OrgId = t.OrgId,
                    Timestamp = t.Timestamp,
                    ReceivedAt = t.ReceivedAt
                }).ToList();

                response.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on EventService.DeviceEventMessage");

                response.ErrorMessage = ex.Message;
                response.IsSuccessful = false;
            }

            return response;
        }

        public async Task<ResponseMessage<EventMessage?>> SaveEventAsync(EventMessage eventMessage)
        {
            var response = new ResponseMessage<EventMessage?>();

            try
            {
                if (eventMessage?.EventId != 109 && eventMessage?.EventId != 110)
                {
                    response.IsSuccessful = false;
                    response.ErrorMessage = "Only EventId 109 or 110 are accepted.";
                    return response;
                }

                Event? @event;

                if (eventMessage.EventId == 109)
                {
                    @event = new Event
                    {
                        Description = eventMessage.Description,
                        DeviceId = eventMessage.DeviceId,
                        OrgId = eventMessage.OrgId,
                        Timestamp = eventMessage.Timestamp,
                        ReceivedAt = eventMessage.ReceivedAt
                    };

                    // Extract and convert latitude
                    if (eventMessage.Measurements.TryGetValue("lat", out var latObj) &&
                        double.TryParse(latObj?.ToString(), out var latitude))
                    {
                        @event.Latitude = latitude;

                        eventMessage.Measurements.Remove("lat");
                    }
                    else
                    {
                        response.IsSuccessful = false;
                        response.ErrorMessage = "Latitude is not available or invalid.";
                        return response;
                    }

                    // Extract and convert longitude
                    if (eventMessage.Measurements.TryGetValue("long", out var longObj) &&
                        double.TryParse(longObj?.ToString(), out var longitude))
                    {
                        @event.Longitude = longitude;

                        eventMessage.Measurements.Remove("long");
                    }
                    else
                    {
                        response.IsSuccessful = false;
                        response.ErrorMessage = "Longitude is not available or invalid.";
                        return response;
                    }

                    @event.Measurements = eventMessage.Measurements;
                }
                else
                {
                    @event = await eventRepository.GetEventByDeviceIdAsync(eventMessage.OrgId, eventMessage.DeviceId);

                    if (@event != null)
                    {
                        @event.Measurements = eventMessage.Measurements;
                        @event.Timestamp = eventMessage.Timestamp;
                        @event.ReceivedAt = eventMessage.ReceivedAt;
                    }
                }

                if (@event != null)
                {
                    var insertResult = await eventRepository.SaveEventAsync(@event);

                    if (insertResult == null)
                    {
                        response.ErrorMessage = "Unable to save device event";
                        response.IsSuccessful = false;
                        return response;
                    }

                    response.Data = new EventMessage
                    {
                        EventId = eventMessage!.EventId,
                        Description = insertResult.Description,
                        Measurements = insertResult.Measurements,
                        DeviceId = insertResult.DeviceId,
                        OrgId = insertResult.OrgId,
                        Timestamp = insertResult.Timestamp,
                        ReceivedAt = insertResult.ReceivedAt
                    };

                    response.IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on EventService.SaveEventAsync");

                response.ErrorMessage = ex.Message;
                response.IsSuccessful = false;
            }

            return response;
        }
    }
}