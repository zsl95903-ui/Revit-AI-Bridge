using System.Collections.Generic;
using System.Linq;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public static class BridgeComponentConfigurationExtensions
{
	public static BridgeComponentConfigurationDto ToDto(this BridgeComponentConfigurationItem item)
	{
		return new BridgeComponentConfigurationDto
		{
			Number = item.Number,
			Station = item.Station,
			BridgeTypeId = item.BridgeType?.Id,
			BridgeTypeName = item.BridgeType?.Name,
			BridgeParameters = item.BridgeParameters,
			FoundationTypeId = item.FoundationType?.Id,
			FoundationTypeName = item.FoundationType?.Name,
			FoundationParameters = item.FoundationParameters,
			PierTypeId = item.PierType?.Id,
			PierTypeName = item.PierType?.Name,
			PierParameters = item.PierParameters,
			BeamTypeId = item.BeamType?.Id,
			BeamTypeName = item.BeamType?.Name,
			BeamParameters = item.BeamParameters,
			BearingTypeId = item.BearingType?.Id,
			BearingTypeName = item.BearingType?.Name,
			BearingParameters = item.BearingParameters
		};
	}

	public static BridgeComponentConfigurationItem ToModel(this BridgeComponentConfigurationDto dto)
	{
		return new BridgeComponentConfigurationItem
		{
			Number = dto.Number,
			Station = dto.Station,
			BridgeParameters = (dto.BridgeParameters ?? string.Empty),
			FoundationParameters = (dto.FoundationParameters ?? string.Empty),
			PierParameters = (dto.PierParameters ?? string.Empty),
			BeamParameters = (dto.BeamParameters ?? string.Empty),
			BearingParameters = (dto.BearingParameters ?? string.Empty)
		};
	}

	public static void SaveConfigurationsToProject(this RoadProject project, IList<BridgeComponentConfigurationItem> configurations)
	{
		BridgeComponentConfigurations bridgeComponentConfigurations = new BridgeComponentConfigurations
		{
			Configurations = configurations.Select((BridgeComponentConfigurationItem c) => c.ToDto()).ToList()
		};
		project.BridgeComponentConfigurations = bridgeComponentConfigurations;
	}

	public static List<BridgeComponentConfigurationItem> LoadConfigurationsFromProject(this RoadProject project, IList<UserDefinedBridgeComponent> bridgeTypes, IList<UserDefinedBridgeComponent> foundationTypes, IList<UserDefinedBridgeComponent> pierTypes, IList<UserDefinedBridgeComponent> beamTypes, IList<UserDefinedBridgeComponent> bearingTypes)
	{
		List<BridgeComponentConfigurationItem> list = new List<BridgeComponentConfigurationItem>();
		if (project.BridgeComponentConfigurations == null || project.BridgeComponentConfigurations.IsEmpty)
		{
			return list;
		}
		foreach (BridgeComponentConfigurationDto dto in project.BridgeComponentConfigurations.Configurations)
		{
			BridgeComponentConfigurationItem bridgeComponentConfigurationItem = dto.ToModel();
			if (!string.IsNullOrEmpty(dto.BridgeTypeId))
			{
				bridgeComponentConfigurationItem.BridgeType = bridgeTypes.FirstOrDefault((UserDefinedBridgeComponent t) => t.Id == dto.BridgeTypeId);
			}
			if (!string.IsNullOrEmpty(dto.FoundationTypeId))
			{
				bridgeComponentConfigurationItem.FoundationType = foundationTypes.FirstOrDefault((UserDefinedBridgeComponent t) => t.Id == dto.FoundationTypeId);
			}
			if (!string.IsNullOrEmpty(dto.PierTypeId))
			{
				bridgeComponentConfigurationItem.PierType = pierTypes.FirstOrDefault((UserDefinedBridgeComponent t) => t.Id == dto.PierTypeId);
			}
			if (!string.IsNullOrEmpty(dto.BeamTypeId))
			{
				bridgeComponentConfigurationItem.BeamType = beamTypes.FirstOrDefault((UserDefinedBridgeComponent t) => t.Id == dto.BeamTypeId);
			}
			if (!string.IsNullOrEmpty(dto.BearingTypeId))
			{
				bridgeComponentConfigurationItem.BearingType = bearingTypes.FirstOrDefault((UserDefinedBridgeComponent t) => t.Id == dto.BearingTypeId);
			}
			list.Add(bridgeComponentConfigurationItem);
		}
		return list;
	}

	public static PilePositionDto ToDto(this PilePositionItem item)
	{
		return new PilePositionDto
		{
			Number = item.Number,
			PierNumber = item.PierNumber,
			PileNumber = item.PileNumber,
			PileTypeId = item.PileType?.Id,
			PileTypeName = item.PileType?.Name,
			XCoordinate = item.XCoordinate,
			YCoordinate = item.YCoordinate,
			PileLength = item.PileLength
		};
	}

	public static PilePositionItem ToModel(this PilePositionDto dto)
	{
		return new PilePositionItem
		{
			Number = dto.Number,
			PierNumber = (dto.PierNumber ?? string.Empty),
			PileNumber = (dto.PileNumber ?? string.Empty),
			XCoordinate = (dto.XCoordinate ?? string.Empty),
			YCoordinate = (dto.YCoordinate ?? string.Empty),
			PileLength = (dto.PileLength ?? string.Empty)
		};
	}

	public static void SavePilePositionsToProject(this RoadProject project, IList<PilePositionItem> positions)
	{
		PilePositionConfigurations pilePositionConfigurations = new PilePositionConfigurations
		{
			Positions = positions.Select((PilePositionItem p) => p.ToDto()).ToList()
		};
		project.PilePositionConfigurations = pilePositionConfigurations;
	}

	public static List<PilePositionItem> LoadPilePositionsFromProject(this RoadProject project, IList<UserDefinedBridgeComponent> pileTypes)
	{
		List<PilePositionItem> list = new List<PilePositionItem>();
		if (project.PilePositionConfigurations == null || project.PilePositionConfigurations.IsEmpty)
		{
			return list;
		}
		foreach (PilePositionDto dto in project.PilePositionConfigurations.Positions)
		{
			PilePositionItem pilePositionItem = dto.ToModel();
			if (!string.IsNullOrEmpty(dto.PileTypeId))
			{
				pilePositionItem.PileType = pileTypes.FirstOrDefault((UserDefinedBridgeComponent t) => t.Id == dto.PileTypeId);
			}
			list.Add(pilePositionItem);
		}
		return list;
	}
}
