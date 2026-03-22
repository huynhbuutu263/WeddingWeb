using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class WeddingCard : BaseEntity
{
    private readonly List<CardImage> _images = new();

    public string Title { get; private set; }
    public string SlugUrl { get; private set; }
    public DateTime EventDate { get; private set; }
    public Guid TemplateId { get; private set; }
    public Template Template { get; private set; }
    public IReadOnlyCollection<CardImage> Images => _images.AsReadOnly();

    private WeddingCard() { Title = string.Empty; SlugUrl = string.Empty; Template = null!; }

    public WeddingCard(string title, string slugUrl, DateTime eventDate, Guid templateId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(slugUrl))
            throw new ArgumentException("SlugUrl cannot be empty.", nameof(slugUrl));

        Title = title;
        SlugUrl = slugUrl;
        EventDate = eventDate;
        TemplateId = templateId;
        Template = null!; // Populated by EF Core navigation loading
    }

    public void AddImage(string imageUrl)
    {
        _images.Add(new CardImage(Id, imageUrl));
    }
}
