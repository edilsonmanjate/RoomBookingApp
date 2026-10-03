using Microsoft.AspNetCore.Mvc;

using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;

namespace RoomBooking.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class RoomBookingController : ControllerBase
{
    private IRoomBookingRequestProcessor roomBookingRequestProcessor;

    public RoomBookingController(IRoomBookingRequestProcessor roomBookingRequestProcessor)
    {
        this.roomBookingRequestProcessor = roomBookingRequestProcessor;
    }

    [HttpPost]
    public async Task<IActionResult> BookRoom(RoomBookingRequest request)
    {
        if(ModelState.IsValid)
        {
            var result = roomBookingRequestProcessor.BookRoom(request);
            if(result.Flag == BookingResultFlag.Success)
            {
                return Ok(result);
            }
            ModelState.AddModelError(nameof(RoomBookingRequest.Date), "No rooms available for the selected date.");
        }

        return BadRequest(ModelState);
    }
}
