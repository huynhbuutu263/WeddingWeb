using System;
using Xunit;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Domain.UnitTests.Entities;

public class WeddingCardTests
{
    [Fact]
    public void Constructor_WithEmptyStrings_ShouldThrowArgumentException()
    {
        // Arrange
        var eventDate = DateTime.Now.AddMonths(6);
        var templateId = Guid.NewGuid();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeddingCard("", "valid-slug", eventDate, templateId));
        Assert.Throws<ArgumentException>(() => new WeddingCard("Valid Title", "", eventDate, templateId));
        Assert.Throws<ArgumentException>(() => new WeddingCard(null, "valid-slug", eventDate, templateId));
        Assert.Throws<ArgumentException>(() => new WeddingCard("Valid Title", null, eventDate, templateId));
    }
    
    [Fact]
    public void Constructor_ValidInputs_ShouldInitializeProperly()
    {
        // Arrange
        var title = "John & Jane's Wedding";
        var slugUrl = "john-jane-wedding-2026";
        var eventDate = new DateTime(2026, 8, 15);
        var templateId = Guid.NewGuid();
        
        // Act
        var weddingCard = new WeddingCard(title, slugUrl, eventDate, templateId);
        
        // Assert
        Assert.Equal(title, weddingCard.Title);
        Assert.Equal(slugUrl, weddingCard.SlugUrl);
        Assert.Equal(eventDate, weddingCard.EventDate);
        Assert.Equal(templateId, weddingCard.TemplateId);
    }
}
