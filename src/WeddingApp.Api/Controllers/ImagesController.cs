using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeddingApp.Application.Images.Commands;

namespace WeddingApp.Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        var url = await mediator.Send(
            new UploadImageCommand(file.OpenReadStream(), file.FileName), cancellationToken);
        return Ok(new { url });
    }
}
