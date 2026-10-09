using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Common;

public sealed class ElementCollection
{
	public IReadOnlyList<object> Elements { get; }

	public int Count => Elements.Count;

	public ElementCollection(IEnumerable<object> elements)
	{
		Elements = elements.ToList().AsReadOnly();
	}

	public static ElementCollection Empty()
	{
		return new ElementCollection(Enumerable.Empty<object>());
	}

	public static ElementCollection From(IEnumerable<object> elements)
	{
		return new ElementCollection(elements);
	}
}
