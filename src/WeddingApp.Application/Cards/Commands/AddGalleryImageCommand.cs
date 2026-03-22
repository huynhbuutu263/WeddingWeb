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
        var cardExists = await context.WeddingCards
            .AnyAsync(c => c.Id == request.CardId, cancellationToken);

        if (!cardExists)
            throw new NotFoundException(nameof(WeddingCard), request.CardId);

        var image = new CardImage(request.CardId, request.ImageUrl);
        context.CardImages.Add(image);
        await context.SaveChangesAsync(cancellationToken);
    }
}
