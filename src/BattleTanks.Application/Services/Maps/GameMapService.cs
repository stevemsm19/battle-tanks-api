using BattleTanks.Application.DTOs.Maps;
using BattleTanks.Application.Interfaces.Maps;
using BattleTanks.Domain.Interfaces.Maps;
using MapsterMapper;

namespace BattleTanks.Application.Services.Maps;

public class GameMapService : IGameMapService
{
    private readonly IGameMapRepository _mapRepository;
    private readonly IMapper _mapper;

    public GameMapService(IGameMapRepository mapRepository, IMapper mapper)
    {
        _mapRepository = mapRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GameMapResponseDTO>> GetAllMapsAsync()
    {
        var maps = await _mapRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GameMapResponseDTO>>(maps);
    }
}
