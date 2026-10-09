using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class PilePositionConfigurations
{
	public List<PilePositionDto> Positions { get; set; } = new List<PilePositionDto>();

	public bool IsEmpty
	{
		get
		{
			if (Positions != null)
			{
				return Positions.Count == 0;
			}
			return true;
		}
	}
}
