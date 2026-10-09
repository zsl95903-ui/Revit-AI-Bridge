using System.Collections.Generic;

namespace RevitAi.Abstractions.Infrastructure;

public sealed class PilePositionExcelData
{
	public List<PilePositionExcelItem> Positions { get; set; } = new List<PilePositionExcelItem>();
}
