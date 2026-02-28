using Mapster;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Application.DTOs.Users;

namespace BattleTanks.Application.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UpdateUserRequestDTO, User>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.FirebaseUid)
            .Map(dest => dest.Nickname, src => src.Nickname)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.AvatarUrl, src => src.AvatarUrl);

        config.NewConfig<User, UserResponseDTO>();
    }
}