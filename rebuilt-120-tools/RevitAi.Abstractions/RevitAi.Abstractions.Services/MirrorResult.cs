using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Services;

public class MirrorResult
{
	public bool Success { get; set; }

	public IEnumerable<int> MirrorElementIds { get; set; } = Enumerable.Empty<int>();

	public bool IsMirrorCopy { get; set; }
}
