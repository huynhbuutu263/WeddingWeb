using System.Text;
using Microsoft.Extensions.Options;
using WeddingApp.Infrastructure.Services;

namespace WeddingApp.Infrastructure.UnitTests.Services;

public class LocalFileStorageServiceTests
{
    private static LocalFileStorageService CreateService(string uploadsFolder) =>
        new(Options.Create(new StorageSettings { UploadsFolder = uploadsFolder }));

    [Fact]
    public async Task UploadImageAsync_ValidStream_SavesFileAndReturnsUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var service = CreateService(tempDir);
        var content = "fake image content";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        try
        {
            // Act
            var url = await service.UploadImageAsync(stream, "test.jpg");

            // Assert
            Assert.StartsWith("/uploads/", url);
            var filename = Path.GetFileName(url);
            var filePath = Path.Combine(tempDir, filename);
            Assert.True(File.Exists(filePath));

            var savedContent = await File.ReadAllTextAsync(filePath);
            Assert.Equal(content, savedContent);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadImageAsync_CreatesUploadsDirectoryIfNotExists()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Assert.False(Directory.Exists(tempDir));
        var service = CreateService(tempDir);
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        try
        {
            // Act
            await service.UploadImageAsync(stream, "image.png");

            // Assert
            Assert.True(Directory.Exists(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadImageAsync_UniqueFilenamesGeneratedPerCall()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var service = CreateService(tempDir);

        try
        {
            using var stream1 = new MemoryStream(new byte[] { 1 });
            using var stream2 = new MemoryStream(new byte[] { 2 });

            // Act
            var url1 = await service.UploadImageAsync(stream1, "photo.jpg");
            var url2 = await service.UploadImageAsync(stream2, "photo.jpg");

            // Assert
            Assert.NotEqual(url1, url2);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }
}
