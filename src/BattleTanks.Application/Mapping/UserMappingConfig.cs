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
            .IgnoreNullValues(true);
    }
}