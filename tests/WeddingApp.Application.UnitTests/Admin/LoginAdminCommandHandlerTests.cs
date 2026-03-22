using Microsoft.EntityFrameworkCore;
using Moq;
using WeddingApp.Application.Admin.Commands;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Admin;

public class LoginAdminCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        using var context = CreateContext();
        const string email = "admin@example.com";
        const string password = "secret";
        const string hash = "hashed_secret";
        const string expectedToken = "jwt.token.value";

        var user = new User(email, hash, "Admin", "User");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Verify(password, hash)).Returns(true);

        var mockJwt = new Mock<IJwtProvider>();
        mockJwt.Setup(j => j.Generate(It.IsAny<User>())).Returns(expectedToken);

        var handler = new LoginAdminCommandHandler(context, mockHasher.Object, mockJwt.Object);

        // Act
        var token = await handler.Handle(new LoginAdminCommand(email, password), CancellationToken.None);

        // Assert
        Assert.Equal(expectedToken, token);
        mockJwt.Verify(j => j.Generate(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsNotFoundException()
    {
        // Arrange
        using var context = CreateContext();
        const string email = "admin@example.com";

        var user = new User(email, "hashed", "Admin", "User");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var mockJwt = new Mock<IJwtProvider>();

        var handler = new LoginAdminCommandHandler(context, mockHasher.Object, mockJwt.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new LoginAdminCommand(email, "wrong-password"), CancellationToken.None));

        mockJwt.Verify(j => j.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsNotFoundException()
    {
        // Arrange
        using var context = CreateContext();
        var mockHasher = new Mock<IPasswordHasher>();
        var mockJwt = new Mock<IJwtProvider>();
        var handler = new LoginAdminCommandHandler(context, mockHasher.Object, mockJwt.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new LoginAdminCommand("nobody@example.com", "pass"), CancellationToken.None));
    }
}
