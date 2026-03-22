using MediatR;
using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Application.Templates.Queries;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Cards.Queries;

public record CardDto(
    Guid Id,
    string Title,
    string SlugUrl,
    DateTime EventDate,
    TemplateDto Template,
    IEnumerable<string> ImageUrls);

public record GetCardBySlugQuery(string Slug) : IRequest<CardDto>;

public class GetCardBySlugQueryHandler(IAppDbContext context) : IRequestHandler<GetCardBySlugQuery, CardDto>
{
    public async Task<CardDto> Handle(GetCardBySlugQuery request, CancellationToken cancellationToken)
    {
        var card = await context.WeddingCards
            .AsNoTracking()
            .Include(c => c.Template)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.SlugUrl == request.Slug, cancellationToken);

        if (card is null)
            throw new NotFoundException(nameof(WeddingCard), request.Slug);

        return new CardDto(
            card.Id,
            card.Title,
            card.SlugUrl,
            card.EventDate,
            new TemplateDto(card.Template.Id, card.Template.Name, card.Template.HtmlStructure),
            card.Images.Select(i => i.Url));
    }
}
