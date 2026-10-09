using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class WallAlignmentService
{
	private readonly IGeometryService _geometryService;

	public WallAlignmentService(IGeometryService geometryService)
	{
		_geometryService = geometryService ?? throw new ArgumentNullException("geometryService");
	}

	public WallFloorAlignmentResult AlignWallsToFloors(object document, IEnumerable<object> walls, IEnumerable<object> floors)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Expected O, but got Unknown
		if (document == null)
		{
			LogError("AlignWallsToFloors: document 参数为空");
			return new WallFloorAlignmentResult(0, 0, new List<string> { "文档对象为空" });
		}
		if (walls == null)
		{
			LogError("AlignWallsToFloors: walls 参数为空");
			return new WallFloorAlignmentResult(0, 0, new List<string> { "墙集合为空" });
		}
		if (floors == null)
		{
			LogError("AlignWallsToFloors: floors 参数为空");
			return new WallFloorAlignmentResult(0, 0, new List<string> { "楼板集合为空" });
		}
		int num = 0;
		int num2 = 0;
		List<string> list = new List<string>();
		List<object> list2 = walls.ToList();
		List<object> list3 = floors.ToList();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
		defaultInterpolatedStringHandler.AppendLiteral("开始处理 ");
		defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 面墙和 ");
		defaultInterpolatedStringHandler.AppendFormatted(list3.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个楼板");
		LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		foreach (object item in list2)
		{
			try
			{
				object obj = FindMatchingFloor(item, list3);
				if (obj == null)
				{
					int elementId = GetElementId(item);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(12, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("墙 ");
					defaultInterpolatedStringHandler2.AppendFormatted(elementId);
					defaultInterpolatedStringHandler2.AppendLiteral(": 未找到匹配的楼板");
					list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
					num2++;
					continue;
				}
				double? floorBottomLevel = _geometryService.GetFloorBottomLevel(obj);
				if (!floorBottomLevel.HasValue)
				{
					int elementId2 = GetElementId(item);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(12, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("墙 ");
					defaultInterpolatedStringHandler3.AppendFormatted(elementId2);
					defaultInterpolatedStringHandler3.AppendLiteral(": 无法获取楼板标高");
					list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
					num2++;
				}
				else if (_geometryService.ModifyWallTopLevel(document, item, floorBottomLevel.Value))
				{
					num++;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(11, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("成功对齐墙 ");
					defaultInterpolatedStringHandler4.AppendFormatted(GetElementId(item));
					defaultInterpolatedStringHandler4.AppendLiteral(" 到楼板 ");
					defaultInterpolatedStringHandler4.AppendFormatted(GetElementId(obj));
					LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
				}
				else
				{
					int elementId3 = GetElementId(item);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(11, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("墙 ");
					defaultInterpolatedStringHandler5.AppendFormatted(elementId3);
					defaultInterpolatedStringHandler5.AppendLiteral(": 修改墙顶部失败");
					list.Add(defaultInterpolatedStringHandler5.ToStringAndClear());
					num2++;
				}
			}
			catch (Exception ex)
			{
				int elementId4 = GetElementId(item);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(4, 2);
				defaultInterpolatedStringHandler6.AppendLiteral("墙 ");
				defaultInterpolatedStringHandler6.AppendFormatted(elementId4);
				defaultInterpolatedStringHandler6.AppendLiteral(": ");
				defaultInterpolatedStringHandler6.AppendFormatted(ex.Message);
				string text = defaultInterpolatedStringHandler6.ToStringAndClear();
				list.Add(text);
				num2++;
				LogError(text);
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(14, 2);
		defaultInterpolatedStringHandler7.AppendLiteral("对齐完成: 成功 ");
		defaultInterpolatedStringHandler7.AppendFormatted(num);
		defaultInterpolatedStringHandler7.AppendLiteral(", 失败 ");
		defaultInterpolatedStringHandler7.AppendFormatted(num2);
		LogInfo(defaultInterpolatedStringHandler7.ToStringAndClear());
		return new WallFloorAlignmentResult(num, num2, list);
	}

	private object? FindMatchingFloor(object wall, IEnumerable<object> floors)
	{
		try
		{
			return floors.FirstOrDefault((object floor) => _geometryService.IsWallUnderFloor(wall, floor));
		}
		catch (Exception ex)
		{
			LogError("FindMatchingFloor 失败: " + ex.Message);
			return null;
		}
	}

	private static int GetElementId(object element)
	{
		try
		{
			object obj = ((element is Element) ? element : null);
			return (int)((obj != null) ? ((Element)obj).Id.Value : (-1L));
		}
		catch
		{
			return -1;
		}
	}

	private void LogError(string message)
	{
		try
		{
			Logger.Error("[WallAlignmentService] " + message);
		}
		catch
		{
		}
	}

	private void LogInfo(string message)
	{
		try
		{
			Logger.Info("[WallAlignmentService] " + message);
		}
		catch
		{
		}
	}
}
