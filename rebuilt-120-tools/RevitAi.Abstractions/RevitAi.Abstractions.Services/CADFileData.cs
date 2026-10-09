using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public sealed class CADFileData
{
	public CADFileInfo FileInfo { get; set; } = new CADFileInfo();

	public List<CADLayerInfo> Layers { get; set; } = new List<CADLayerInfo>();

	public List<CADTextInfo> Texts { get; set; } = new List<CADTextInfo>();

	public List<CADBlockInfo> Blocks { get; set; } = new List<CADBlockInfo>();

	public List<CADLineInfo> Lines { get; set; } = new List<CADLineInfo>();

	public List<CADTextInfo> GetTextsByLayer(string layerName)
	{
		return Texts.FindAll((CADTextInfo t) => t.LayerName == layerName);
	}

	public List<CADBlockInfo> GetBlocksByLayer(string layerName)
	{
		return Blocks.FindAll((CADBlockInfo b) => b.LayerName == layerName);
	}

	public List<CADLineInfo> GetLinesByLayer(string layerName)
	{
		return Lines.FindAll((CADLineInfo l) => l.LayerName == layerName);
	}
}
