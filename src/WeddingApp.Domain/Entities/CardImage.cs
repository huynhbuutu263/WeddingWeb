using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class CardImage : BaseEntity
{
    public Guid WeddingCardId { get; private set; }
    public string Url { get; private set; }

    private CardImage() { Url = string.Empty; }

    public CardImage(Guid weddingCardId, string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url cannot be empty.", nameof(url));

        WeddingCardId = weddingCardId;
        Url = url;
    }
}
