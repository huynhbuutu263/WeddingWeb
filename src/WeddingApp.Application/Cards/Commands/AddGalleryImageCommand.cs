using MediatR;
using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Cards.Commands;

public record AddGalleryImageCommand(Guid CardId, string ImageUrl) : IRequest;

public class AddGalleryImageCommandHandler(IAppDbContext context) : IRequestHandler<AddGalleryImageCommand>
{
    public async Task Handle(AddGalleryImageCommand request, CancellationToken cancellationToken)
    {
        var card = await context.WeddingCards
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == request.CardId, cancellationToken);

        if (card is null)
            throw new NotFoundException(nameof(WeddingCard), request.CardId);

        card.AddImage(request.ImageUrl);
        await context.SaveChangesAsync(cancellationToken);
    }
}
