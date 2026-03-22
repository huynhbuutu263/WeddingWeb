using MediatR;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Admin.Commands;

public record RegisterAdminCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<Guid>;

public class RegisterAdminCommandHandler(IAppDbContext context, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterAdminCommand, Guid>
{
    public async Task<Guid> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = passwordHasher.Hash(request.Password);
        var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}
