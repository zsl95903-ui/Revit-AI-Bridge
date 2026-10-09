namespace RevitAi.Abstractions.Models;

public class TileDownloadProgressEventArgs
{
	public int TotalTiles { get; set; }

	public int CompletedTiles { get; set; }

	public int FailedTiles { get; set; }

	public int SuccessTiles { get; set; }

	public TileCoordinate? CurrentTile { get; set; }

	public string? Error { get; set; }

	public double Progress
	{
		get
		{
			if (TotalTiles <= 0)
			{
				return 0.0;
			}
			return (double)CompletedTiles / (double)TotalTiles * 100.0;
		}
	}

	public double SuccessRate
	{
		get
		{
			if (CompletedTiles <= 0)
			{
				return 0.0;
			}
			return (double)SuccessTiles / (double)CompletedTiles * 100.0;
		}
	}
}
