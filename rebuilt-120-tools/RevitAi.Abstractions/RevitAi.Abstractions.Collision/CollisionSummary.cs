namespace RevitAi.Abstractions.Collision;

public class CollisionSummary
{
	public int TotalPairs { get; set; }

	public int CollisionCount { get; set; }

	public int NonCollisionCount { get; set; }

	public double CollisionRate { get; set; }
}
