using Application.Command;
using Application.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.ApiModels.Request;

namespace Web.Controllers;

[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IMediator _mediator;
    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("/{id}")]
    public async Task<ActionResult> GetReservation(string id)
    {
        var result = await _mediator.Send(new GetReservation(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [Route("/reservation/availability")]
    public async Task<ActionResult> AvailabilityCheck(AvailabilityCheckRequest request)
    {
        var result = await _mediator.Send(new AvailabilityCheckCommand(request.Starts, request.Ends));
        return Ok(result);
    }

    [HttpPost]
    [Route("/reservation")]
    public async Task<ActionResult> CreateReservation(ReservationRequest reservation)
    {
        if (string.IsNullOrWhiteSpace(reservation.UserIdentification))
            return BadRequest();

        var result = await _mediator.Send(new PlaceReservationCommand(reservation.UserIdentification, reservation.Starts, reservation.Ends));

        if (result?.Errors == null || result.Errors.Any())
            new ObjectResult(result) { StatusCode = StatusCodes.Status400BadRequest };

        return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
    }

    [HttpPut]
    [Route("/reservation")]
    public async Task<ActionResult> EditReservation(ModifyReservationRequest reservation)
    {
        if (string.IsNullOrWhiteSpace(reservation.ReservationId))
            return BadRequest();

        var result = await _mediator.Send(new PlaceReservationCommand(reservation.Starts, reservation.Ends, reservation.ReservationId));

        if (result?.Errors == null || result.Errors.Any())
            new ObjectResult(result) { StatusCode = StatusCodes.Status400BadRequest };

        return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
    }

    [HttpDelete]
    [Route("/reservation/{id}")]
    public async Task<ActionResult> ReservationCancelAsync(string id)
    {
        var result = await _mediator.Send(new CancelReservationCommand(id));

        if (!result)
            return NotFound();

        return Ok();
    }

}