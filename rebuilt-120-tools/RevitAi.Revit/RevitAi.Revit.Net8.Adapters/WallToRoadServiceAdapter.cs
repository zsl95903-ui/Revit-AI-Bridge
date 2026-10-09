using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Revit.WallToRoad;
using RevitAi.Abstractions.Services;
using RevitAi.Revit.Services;
using Autodesk.Revit.DB;
using ns6;

namespace RevitAi.Revit.Net8.Adapters;

public sealed class WallToRoadServiceAdapter : IWallToRoadService
{
	private readonly WallToRoadService wallToRoadService_0;

	public WallToRoadServiceAdapter(WallToRoadService service)
	{
		wallToRoadService_0 = service ?? throw new ArgumentNullException("service");
	}

	public Result<List<int>> CreateRoadFromWalls(object document, WallToRoadRequest request)
	{
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return Result<List<int>>.Failure("文档对象无效");
		}
		return wallToRoadService_0.CreateRoadFromWalls(val, request);
	}

	public Result<List<int>> CreateRoadFromWallsWithTransaction(object document, WallToRoadRequest request, object externalTransaction)
	{
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return Result<List<int>>.Failure("文档对象无效");
		}
		Transaction val2 = (Transaction)((externalTransaction is Transaction) ? externalTransaction : null);
		if (val2 == null)
		{
			return Result<List<int>>.Failure("事务对象无效");
		}
		return wallToRoadService_0.CreateRoadFromWallsWithTransaction(val, request, val2);
	}
}
