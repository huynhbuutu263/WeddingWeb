using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeddingApp.Application.Cards.Commands;
using WeddingApp.Application.Cards.Queries;

namespace WeddingApp.Api.Controllers;

[ApiController]
[Route("api/cards")]
public class CardsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCard(CreateCardCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBySlug), new { slug = command.SlugUrl }, new { id });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var card = await mediator.Send(new GetCardBySlugQuery(slug), cancellationToken);
        return Ok(card);
    }
}
