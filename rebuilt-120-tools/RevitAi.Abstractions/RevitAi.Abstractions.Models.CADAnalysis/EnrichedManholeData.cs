using System.Collections.Generic;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class EnrichedManholeData
{
	public ManholeData CADData { get; set; }

	public ManholeParameterData? ParameterData { get; set; }

	public string? MatchedAnnotation { get; set; }

	public double PositionX { get; set; }

	public double PositionY { get; set; }

	public double PositionZ { get; set; }

	public double WellDepthMM { get; set; }

	public double PipeBottomElevationMM { get; set; }

	public List<string> MissingFields { get; set; } = new List<string>();

	public bool HasCompleteData
	{
		get
		{
			if (ParameterData != null && ParameterData.HasValidElevation() && ParameterData.HasValidWellDepth())
			{
				return MissingFields.Count == 0;
			}
			return false;
		}
	}

	public string ManholeId
	{
		get
		{
			string text;
			if (string.IsNullOrEmpty(ParameterData?.ManholeId))
			{
				if (!string.IsNullOrEmpty(MatchedAnnotation))
				{
					return MatchedAnnotation;
				}
				text = CADData.GetAllAnnotationsText();
				if (text == null)
				{
					return "未标注";
				}
			}
			else
			{
				text = ParameterData.ManholeId;
			}
			return text;
		}
	}

	public void CheckMissingFields()
	{
		if (ParameterData == null)
		{
			MissingFields.Add("无参数表数据");
			return;
		}
		if (ParameterData.GroundElevationM == 0.0)
		{
			MissingFields.Add("地面标高");
		}
		if (ParameterData.PipeBottomElevationM == 0.0)
		{
			MissingFields.Add("管底标高");
		}
		if (ParameterData.WellDepthM == 0.0)
		{
			MissingFields.Add("井深");
		}
	}
}
