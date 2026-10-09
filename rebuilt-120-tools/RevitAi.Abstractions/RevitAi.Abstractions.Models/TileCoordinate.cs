namespace RevitAi.Abstractions.Models;

public class TileCoordinate
{
	public int X { get; set; }

	public int Y { get; set; }

	public int Zoom { get; set; }

	public string Url { get; set; }

	public string LocalPath { get; set; } = string.Empty;

	public bool IsDownloaded { get; set; }

	public int RetryCount { get; set; }

	public TileCoordinate(int x, int y, int zoom, string url)
	{
		X = x;
		Y = y;
		Zoom = zoom;
		Url = url;
		IsDownloaded = false;
		RetryCount = 0;
	}

	public (int pixelX, int pixelY) GetPixelPosition(int minX, int minY, int tileSize = 256)
	{
		int item = (X - minX) * tileSize;
		int item2 = (Y - minY) * tileSize;
		return (pixelX: item, pixelY: item2);
	}

	public override string ToString()
	{
		return $"Tile({X}, {Y}) at Zoom {Zoom}";
	}

	public override bool Equals(object? obj)
	{
		if (obj is TileCoordinate tileCoordinate)
		{
			if (X == tileCoordinate.X && Y == tileCoordinate.Y)
			{
				return Zoom == tileCoordinate.Zoom;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((17 * 31 + X.GetHashCode()) * 31 + Y.GetHashCode()) * 31 + Zoom.GetHashCode();
	}
}
