using ClimaCloud.Core.Interfaces;
using ClimaCloud.Shared.Messages;
using Microsoft.AspNetCore.Mvc;

namespace ClimaCloud.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [Route("SaveEvent")]
        [HttpPost]
        public async Task<IActionResult> SaveEvent(EventMessage eventMessage)
        {
            var deviceEventResponseMessage = await _eventService.SaveEventAsync(eventMessage);

            if (deviceEventResponseMessage?.IsSuccessful == false)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("GetAllEvents")]
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var deviceEventResponseMessage = await _eventService.GetAllEventsAsync();

            if (deviceEventResponseMessage?.IsSuccessful == false)
            {
                return BadRequest(deviceEventResponseMessage);
            }

            return Ok(deviceEventResponseMessage);
        }

        [Route("GetAllEventsByDevice")]
        [HttpGet]
        public async Task<IActionResult> GetAllEventsByDevice(string orgId, string deviceId)
        {
            var deviceEventResponseMessage = await _eventService.GetAllEventsByDeviceAsync(orgId, deviceId);

            if (deviceEventResponseMessage?.IsSuccessful == false)
            {
                return BadRequest(deviceEventResponseMessage);
            }

            return Ok(deviceEventResponseMessage);
        }
    }
}