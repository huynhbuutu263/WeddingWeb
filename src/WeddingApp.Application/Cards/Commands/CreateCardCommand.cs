using MediatR;
using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Cards.Commands;

public record CreateCardCommand(string Title, string SlugUrl, DateTime EventDate, Guid TemplateId) : IRequest<Guid>;

public class CreateCardCommandHandler(IAppDbContext context) : IRequestHandler<CreateCardCommand, Guid>
{
    public async Task<Guid> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var templateExists = await context.Templates
            .AnyAsync(t => t.Id == request.TemplateId, cancellationToken);

        if (!templateExists)
            throw new NotFoundException(nameof(Template), request.TemplateId);

        var card = new WeddingCard(request.Title, request.SlugUrl, request.EventDate, request.TemplateId);
        context.WeddingCards.Add(card);
        await context.SaveChangesAsync(cancellationToken);
        return card.Id;
    }
}
