using MediatR;
using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Interfaces;

namespace WeddingApp.Application.Templates.Queries;

public record TemplateDto(Guid Id, string Name, string HtmlStructure);

public record GetTemplatesQuery : IRequest<IEnumerable<TemplateDto>>;

public class GetTemplatesQueryHandler(IAppDbContext context) : IRequestHandler<GetTemplatesQuery, IEnumerable<TemplateDto>>
{
    public async Task<IEnumerable<TemplateDto>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
    {
        return await context.Templates
            .AsNoTracking()
            .Select(t => new TemplateDto(t.Id, t.Name, t.HtmlStructure))
            .ToListAsync(cancellationToken);
    }
}
