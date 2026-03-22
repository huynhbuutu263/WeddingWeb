using MediatR;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Templates.Commands;

public record CreateTemplateCommand(string Name, string HtmlStructure) : IRequest<Guid>;

public class CreateTemplateCommandHandler(IAppDbContext context) : IRequestHandler<CreateTemplateCommand, Guid>
{
    public async Task<Guid> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new Template(request.Name, request.HtmlStructure);
        context.Templates.Add(template);
        await context.SaveChangesAsync(cancellationToken);
        return template.Id;
    }
}
