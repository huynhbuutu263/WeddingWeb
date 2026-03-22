using Microsoft.Extensions.Options;
using WeddingApp.Application.Common.Interfaces;

namespace WeddingApp.Infrastructure.Services;

public class StorageSettings
{
    public string UploadsFolder { get; set; } = "wwwroot/uploads";
}

public class LocalFileStorageService(IOptions<StorageSettings> options) : IFileStorageService
{
    private readonly string _uploadsFolder = options.Value.UploadsFolder;

    public async Task<string> UploadImageAsync(Stream data, string filename, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_uploadsFolder);

        var uniqueFilename = $"{Guid.NewGuid():N}_{filename}";
        var filePath = Path.Combine(_uploadsFolder, uniqueFilename);

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await data.CopyToAsync(fileStream, cancellationToken);

        return $"/uploads/{uniqueFilename}";
    }
}
