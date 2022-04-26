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
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Route("/reservation/{id}")]
    public async Task<ActionResult> GetReservation(string id)
    {
        var result = await _mediator.Send(new GetReservation(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [Route("/reservation/availability")]
    public async Task<ActionResult> AvailabilityCheck(AvailabilityCheckRequest request)
    {
        var result = await _mediator.Send(new AvailabilityCheckCommand(request.Starts, request.Ends));

        if (result == null)
            return BadRequest();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [Route("/reservation")]
    public async Task<ActionResult> CreateReservation([FromBody] ReservationRequest reservation)
    {
        if (string.IsNullOrWhiteSpace(reservation.UserIdentification))
            return BadRequest();

        var result = await _mediator.Send(new PlaceReservationCommand(reservation.UserIdentification, reservation.Starts, reservation.Ends));

        if (result?.Errors == null || result.Errors.Any())
            return new ObjectResult(result) { StatusCode = StatusCodes.Status400BadRequest };

        return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
    }

    [HttpPut]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [Route("/reservation")]
    public async Task<ActionResult> EditReservation([FromBody] ModifyReservationRequest reservation)
    {
        if (string.IsNullOrWhiteSpace(reservation.ReservationId))
            return BadRequest();

        var result = await _mediator.Send(new PlaceReservationCommand(reservation.Starts, reservation.Ends, reservation.ReservationId));

        if (result?.Errors == null || result.Errors.Any())
            return new ObjectResult(result) { StatusCode = StatusCodes.Status400BadRequest };

        return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
    }

    [HttpDelete]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Route("/reservation/{id}")]
    public async Task<ActionResult> ReservationCancel(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        var result = await _mediator.Send(new CancelReservationCommand(id));

        if (!result)
            return NotFound();

        return Ok();
    }

}