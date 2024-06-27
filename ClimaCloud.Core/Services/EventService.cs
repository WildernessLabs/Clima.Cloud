using ClimaCloud.Core.Entities;
using ClimaCloud.Core.Interfaces;
using ClimaCloud.Shared.Messages;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClimaCloud.Core.Services
{
    public class EventService(IEventRepository eventRepository, ILogger<EventService> logger, IElasticLowLevelClient elasticsearchClient, IConfiguration configuration) : IEventService
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

        public async Task<ResponseMessage<List<BaseEventMessage>>> GetAllEventsByDeviceAsync(string orgId, string deviceId)
        {
            var response = new ResponseMessage<List<BaseEventMessage>>();

            try
            {
                var queryFilters = new Dictionary<string, List<string>>();

                queryFilters.Add("orgId", new List<string> { orgId });

                queryFilters.Add("deviceId.keyword", new List<string> { deviceId });

                var query = new
                {
                    query = new
                    {
                        @bool = new
                        {
                            filter = queryFilters?.Select(term => new
                            {
                                terms = new Dictionary<string, List<string>>
                                        {
                                            { term.Key, term.Value }
                                        }
                            })
                        }
                    },
                    sort = new object[]
                    {
                    new
                    {
                        timestamp = new
                        {
                            order = "desc"
                        }
                    }
                    }
                };

                var searchResponse = await elasticsearchClient.SearchAsync<StringResponse>(configuration["Elasticsearch:Event-Index"], PostData.Serializable(query));

                if (searchResponse?.Success == true)
                {
                    using (JsonDocument document = JsonDocument.Parse(searchResponse.Body))
                    {
                        var hits = document.RootElement.GetProperty("hits").GetProperty("hits");
                        response.Data = new List<BaseEventMessage>();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        foreach (JsonElement hit in hits.EnumerateArray())
                        {
                            var id = hit.GetProperty("_id").GetString();
                            var record = hit.GetProperty("_source").Clone();
                            var @event = JsonSerializer.Deserialize<BaseEventMessage>(record.GetRawText(), options);

                            if (@event != null)
                            {
                                response.Data.Add(@event);
                            }
                        }
                    }

                    response.IsSuccessful = true;
                }
                else
                {
                    logger.LogError($"Error querying Elasticsearch: {searchResponse?.Body}");

                    response.IsSuccessful = false;
                    response.ErrorMessage = $"Failed to search documents: {searchResponse?.Body}";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on EventService.GetAllEventsByDeviceAsync");

                response.ErrorMessage = ex.Message;
                response.IsSuccessful = false;
            }

            return response;
        }
    }
}