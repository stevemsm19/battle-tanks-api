using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Application.DTOs.Rooms;
using Mapster;

namespace BattleTanks.Application.Mapping;

public class RoomMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateRoomRequestDTO, Room>();

        config.NewConfig<Room, RoomResponseDTO>()
            .Map(dest => dest.CurrentPlayers, src => src.RoomUsers.Count)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.GameMapName, src => src.GameMap != null ? src.GameMap.Name : string.Empty);

        config.NewConfig<Room, RoomWithPlayersResponseDTO>()
            .Map(dest => dest.Room, src => src)
            .Map(dest => dest.Players, src => src.RoomUsers);

        config.NewConfig<RoomUser, RoomPlayerResponseDTO>()
            .Map(dest => dest.Id, src => src.User.Id.ToString())
            .Map(dest => dest.ExternalId, src => src.User.ExternalId)
            .Map(dest => dest.Name, src => src.User.Nickname);
    }
}

