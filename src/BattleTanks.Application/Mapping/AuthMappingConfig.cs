using BattleTanks.Domain.Entities.Users;
using BattleTanks.Application.DTOs.Auth;
using Mapster;

namespace BattleTanks.Application.Mapping;

public class AuthMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequestDTO, User>()
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.AvatarUrl, src => string.Empty)
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);
    }
}
