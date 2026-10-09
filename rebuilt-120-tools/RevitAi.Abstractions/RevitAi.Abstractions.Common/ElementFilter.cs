using System;
using System.Collections.Generic;

namespace RevitAi.Abstractions.Common;

public sealed class ElementFilter
{
	public string? CategoryName { get; private set; }

	public string? TypeName { get; private set; }

	public IEnumerable<int>? ElementIds { get; private set; }

	public IEnumerable<string>? UniqueIds { get; private set; }

	public Func<object, bool>? CustomPredicate { get; private set; }

	private ElementFilter()
	{
	}

	public static ElementFilter Empty()
	{
		return new ElementFilter();
	}

	public static ElementFilter ByCategory(string categoryName)
	{
		return new ElementFilter
		{
			CategoryName = categoryName
		};
	}

	public static ElementFilter ByType(string typeName)
	{
		return new ElementFilter
		{
			TypeName = typeName
		};
	}

	public static ElementFilter ByElementIds(IEnumerable<int> elementIds)
	{
		return new ElementFilter
		{
			ElementIds = elementIds
		};
	}

	public static ElementFilter ByCustom(Func<object, bool> predicate)
	{
		return new ElementFilter
		{
			CustomPredicate = predicate
		};
	}
}
