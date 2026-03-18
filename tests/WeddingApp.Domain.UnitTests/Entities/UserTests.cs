using WeddingApp.Domain.Entities;

namespace WeddingApp.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesUserAndGeneratesId()
    {
        // Arrange
        var email = "admin@example.com";
        var hash = "hashedpass";
        
        // Act
        var user = new User(email, hash, "John", "Doe");

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_EmptyEmail_ThrowsArgumentException(string? invalidEmail)
    {
        // Arrange
        var hash = "hashedpass";

        // Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Throws<ArgumentException>(() => new User(invalidEmail, hash, "John", "Doe"));
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
