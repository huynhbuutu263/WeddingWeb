using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Common.Interfaces;

public interface IJwtProvider
{
    string Generate(User user);
}
