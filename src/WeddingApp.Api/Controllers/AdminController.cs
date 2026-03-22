using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingApp.Application.Admin.Commands;
using WeddingApp.Application.Admin.Queries;

namespace WeddingApp.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterAdminCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginAdminCommand command, CancellationToken cancellationToken)
    {
        var token = await mediator.Send(command, cancellationToken);
        return Ok(new { token });
    }

    [HttpGet("dashboard")]
    [Authorize]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var stats = await mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
        return Ok(stats);
    }
}
