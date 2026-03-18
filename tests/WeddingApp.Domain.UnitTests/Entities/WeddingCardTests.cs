using System;
using Xunit;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Domain.UnitTests.Entities;

public class WeddingCardTests
{
    [Fact]
    public void Constructor_WithEmptyStrings_ShouldThrowArgumentException()
    {
        // COPILOT TASK:
        // Arrange
        // Try to instantiate a WeddingCard with an empty title or slug.
        
        // Act & Assert
        // Assert.Throws<ArgumentException>(() => new WeddingCard(...));
    }
    
    [Fact]
    public void Constructor_ValidInputs_ShouldInitializeProperly()
    {
        // COPILOT TASK:
        // Instantiate correctly and assert properties are set.
    }
}
