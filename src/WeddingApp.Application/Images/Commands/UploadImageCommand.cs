using MediatR;
using WeddingApp.Application.Common.Interfaces;

namespace WeddingApp.Application.Images.Commands;

public record UploadImageCommand(Stream Data, string Filename) : IRequest<string>;

public class UploadImageCommandHandler(IFileStorageService fileStorage) : IRequestHandler<UploadImageCommand, string>
{
    public async Task<string> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        return await fileStorage.UploadImageAsync(request.Data, request.Filename, cancellationToken);
    }
}
