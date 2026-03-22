using Microsoft.EntityFrameworkCore;
using Moq;
using WeddingApp.Application.Admin.Commands;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Admin;

public class RegisterAdminCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ValidCommand_SavesUserAndReturnsId()
    {
        // Arrange
        using var context = CreateContext();
        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");

        var handler = new RegisterAdminCommandHandler(context, mockHasher.Object);
        var command = new RegisterAdminCommand("admin@example.com", "PlainPassword1!", "John", "Admin");

        // Act
        var id = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, id);
        var saved = await context.Users.FindAsync(id);
        Assert.NotNull(saved);
        Assert.Equal("admin@example.com", saved.Email);
    }

    [Fact]
    public async Task Handle_ValidCommand_StoresHashedPasswordNotPlaintext()
    {
        // Arrange
        using var context = CreateContext();
        const string plainPassword = "PlainPassword1!";
        const string hashedPassword = "bcrypt_hashed_value";

        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Hash(plainPassword)).Returns(hashedPassword);

        var handler = new RegisterAdminCommandHandler(context, mockHasher.Object);
        var command = new RegisterAdminCommand("admin@example.com", plainPassword, "John", "Admin");

        // Act
        var id = await handler.Handle(command, CancellationToken.None);

        // Assert
        var saved = await context.Users.FindAsync(id);
        Assert.NotNull(saved);
        Assert.NotEqual(plainPassword, saved.PasswordHash);
        Assert.Equal(hashedPassword, saved.PasswordHash);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsHasherExactlyOnce()
    {
        // Arrange
        using var context = CreateContext();
        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");

        var handler = new RegisterAdminCommandHandler(context, mockHasher.Object);
        var command = new RegisterAdminCommand("admin@example.com", "password", "Jane", "Doe");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockHasher.Verify(h => h.Hash("password"), Times.Once);
    }
}
