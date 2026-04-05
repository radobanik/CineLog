using CineLog.Application.Features.People;
using CineLog.Application.Features.People.CreatePerson;
using CineLog.Application.Features.People.DeletePerson;
using CineLog.Application.Features.People.GetPerson;
using CineLog.Application.Features.People.UpdatePerson;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/people")]
public class PeopleController : ControllerBase
{
    private readonly ISender _sender;

    public PeopleController(ISender sender) => _sender = sender;

    /// <summary>Get person by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PersonDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDetailResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetPersonQuery(id), ct));

    /// <summary>Create a new person.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePersonCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>Update a person.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonCommand command, CancellationToken ct)
    {
        await _sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    /// <summary>Delete a person.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeletePersonCommand(id), ct);
        return NoContent();
    }
}
