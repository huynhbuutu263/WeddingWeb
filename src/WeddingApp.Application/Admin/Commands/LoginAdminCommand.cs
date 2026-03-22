using MediatR;
using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Admin.Commands;

public record LoginAdminCommand(string Email, string Password) : IRequest<string>;

public class LoginAdminCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider) : IRequestHandler<LoginAdminCommand, string>
{
    public async Task<string> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new NotFoundException(nameof(User), request.Email);

        return jwtProvider.Generate(user);
    }
}
