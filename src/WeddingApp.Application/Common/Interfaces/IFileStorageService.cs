namespace WeddingApp.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadImageAsync(Stream data, string filename, CancellationToken cancellationToken = default);
}
