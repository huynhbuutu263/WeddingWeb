using MediatR;
using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Interfaces;

namespace WeddingApp.Application.Admin.Queries;

public record DashboardStatsDto(int TotalTemplates, int TotalCards);

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var totalTemplates = await context.Templates.CountAsync(cancellationToken);
        var totalCards = await context.WeddingCards.CountAsync(cancellationToken);
        return new DashboardStatsDto(totalTemplates, totalCards);
    }
}
