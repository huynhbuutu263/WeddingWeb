using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class Guest : BaseEntity
{
    public Guid WeddingCardId { get; private set; }
    public string Name { get; private set; }
    public string? Email { get; private set; }
    public bool IsAttending { get; private set; }

    // Required by ORM
    private Guest() { Name = string.Empty; }

    public Guest(Guid weddingCardId, string name, string? email, bool isAttending)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        WeddingCardId = weddingCardId;
        Name = name;
        Email = email;
        IsAttending = isAttending;
    }
}
