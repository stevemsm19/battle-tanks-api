using BattleTanks.Domain.Entities.Maps;

namespace BattleTanks.Application.DTOs.Maps;

public record GameMapResponseDTO(Guid Id, string Name);