using Moq;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Application.Images.Commands;

namespace WeddingApp.Application.UnitTests.Images;

public class UploadImageCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_DelegatesUploadToFileStorageAndReturnsUrl()
    {
        // Arrange
        var mockStorage = new Mock<IFileStorageService>();
        const string expectedUrl = "/uploads/abc123_photo.jpg";
        mockStorage
            .Setup(s => s.UploadImageAsync(It.IsAny<Stream>(), "photo.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUrl);

        var handler = new UploadImageCommandHandler(mockStorage.Object);
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var command = new UploadImageCommand(stream, "photo.jpg");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedUrl, result);
        mockStorage.Verify(
            s => s.UploadImageAsync(stream, "photo.jpg", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_FileStorageReturnsUrl_ReturnsExactUrl()
    {
        // Arrange
        var mockStorage = new Mock<IFileStorageService>();
        const string url = "/uploads/unique-name_image.png";
        mockStorage
            .Setup(s => s.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(url);

        var handler = new UploadImageCommandHandler(mockStorage.Object);
        using var stream = new MemoryStream();
        var command = new UploadImageCommand(stream, "image.png");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(url, result);
    }
}
