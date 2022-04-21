using Application.Query;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IMediator _mediator;
    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<Schedule> GetAsync(string id)
    {
        var schedule = await _mediator.Send(new GetSchedule(id));
        return schedule;
    }

}