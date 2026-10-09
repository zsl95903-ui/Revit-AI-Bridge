using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_grid_intersection", Category = "元素查询", Description = "计算两条轴线的交点位置。返回单位：毫米", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetGridIntersectionTool : IAITool
{
	private readonly IElementService ielementService_0;

	public string Name => "get_grid_intersection";

	public string Category => "元素查询";

	public string Description => "计算两条轴线的交点位置";

	public string ParametersSchema => "\r\n{\r\n    \"type\": \"object\",\r\n    \"properties\": {\r\n        \"grid1_id\": {\r\n            \"type\": \"integer\",\r\n            \"description\": \"第一条轴网的元素 ID\"\r\n        },\r\n        \"grid2_id\": {\r\n            \"type\": \"integer\",\r\n            \"description\": \"第二条轴网的元素 ID\"\r\n        }\r\n    },\r\n    \"required\": [\"grid1_id\", \"grid2_id\"]\r\n}";

	public GetGridIntersectionTool(IElementService elementService)
	{
		ielementService_0 = elementService ?? throw new ArgumentNullException("elementService");
	}

	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken)
	{
		try
		{
			if (context.Document == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("无法获取当前文档"));
			}
			if (!context.Parameters.TryGetValue("grid1_id", out var value) || value == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("缺少必需参数: grid1_id"));
			}
			if (!context.Parameters.TryGetValue("grid2_id", out var value2) || value2 == null)
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("缺少必需参数: grid2_id"));
			}
			if (!int.TryParse(value.ToString(), out var result))
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("grid1_id 参数格式错误"));
			}
			if (!int.TryParse(value2.ToString(), out var result2))
			{
				return Task.FromResult<AIToolResult>(AIToolResult.Fail("grid2_id 参数格式错误"));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[GetGridIntersection] 计算轴网交点，轴网1 ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(result);
			defaultInterpolatedStringHandler.AppendLiteral("，轴网2 ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(result2);
			Logger.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			object elementById = ielementService_0.GetElementById(context.Document, result);
			object elementById2 = ielementService_0.GetElementById(context.Document, result2);
			if (elementById == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("未找到 ID 为 ");
				defaultInterpolatedStringHandler2.AppendFormatted(result);
				defaultInterpolatedStringHandler2.AppendLiteral(" 的轴网");
				return Task.FromResult<AIToolResult>(AIToolResult.Fail(defaultInterpolatedStringHandler2.ToStringAndClear()));
			}
			if (elementById2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("未找到 ID 为 ");
				defaultInterpolatedStringHandler3.AppendFormatted(result2);
				defaultInterpolatedStringHandler3.AppendLiteral(" 的轴网");
				return Task.FromResult<AIToolResult>(AIToolResult.Fail(defaultInterpolatedStringHandler3.ToStringAndClear()));
			}
			Grid val = (Grid)((elementById is Grid) ? elementById : null);
			if (val != null)
			{
				Grid val2 = (Grid)((elementById2 is Grid) ? elementById2 : null);
				if (val2 != null)
				{
					Curve curve = val.Curve;
					Curve curve2 = val2.Curve;
					if ((GeometryObject)(object)curve == (GeometryObject)null || (GeometryObject)(object)curve2 == (GeometryObject)null)
					{
						return Task.FromResult<AIToolResult>(AIToolResult.Fail("无法获取轴网曲线"));
					}
					(XYZ, bool)? tuple = method_0(curve, curve2);
					if (!tuple.HasValue)
					{
						return Task.FromResult<AIToolResult>(AIToolResult.Fail("两条轴线平行或不相交"));
					}
					(XYZ, bool) value3 = tuple.Value;
					XYZ item = value3.Item1;
					bool item2 = value3.Item2;
					Class179<double, double, double, string, bool> gparam_ = new Class179<double, double, double, string, bool>(Math.Round(item.X * 304.8, 0), Math.Round(item.Y * 304.8, 0), Math.Round(item.Z * 304.8, 0), "millimeters", item2);
					string elementName = ielementService_0.GetElementName(elementById);
					string elementName2 = ielementService_0.GetElementName(elementById2);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(24, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("成功计算轴网交点: ");
					defaultInterpolatedStringHandler4.AppendFormatted(elementName);
					defaultInterpolatedStringHandler4.AppendLiteral(" 与 ");
					defaultInterpolatedStringHandler4.AppendFormatted(elementName2);
					defaultInterpolatedStringHandler4.AppendLiteral(" 的交点（单位：毫米）");
					return Task.FromResult<AIToolResult>(AIToolResult.Ok(defaultInterpolatedStringHandler4.ToStringAndClear(), (object)new Class180<int, string, int, string, Class179<double, double, double, string, bool>>(result, elementName, result2, elementName2, gparam_)));
				}
			}
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("指定的元素不是轴网类型"));
		}
		catch (Exception ex)
		{
			Logger.Error("[GetGridIntersection] 执行失败", ex);
			return Task.FromResult<AIToolResult>(AIToolResult.Fail("计算轴网交点失败: " + ex.Message));
		}
	}

	private (XYZ Point, bool OnBothSegments)? method_0(Curve curve_0, Curve curve_1)
	{
		try
		{
			Line val = (Line)(object)((curve_0 is Line) ? curve_0 : null);
			if (val != null)
			{
				Line val2 = (Line)(object)((curve_1 is Line) ? curve_1 : null);
				if (val2 != null)
				{
					return method_1(val, val2);
				}
			}
			if (curve_0 is Arc || curve_1 is Arc)
			{
				Logger.Warning("[GetGridIntersection] 弧线轴网交点计算功能尚未完善，尝试使用数值方法");
				return method_3(curve_0, curve_1);
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[GetGridIntersection] 计算交点失败", ex);
			return null;
		}
	}

	private (XYZ Point, bool OnBothSegments)? method_1(Line line_0, Line line_1)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		try
		{
			XYZ endPoint = ((Curve)line_0).GetEndPoint(0);
			XYZ endPoint2 = ((Curve)line_0).GetEndPoint(1);
			XYZ endPoint3 = ((Curve)line_1).GetEndPoint(0);
			XYZ endPoint4 = ((Curve)line_1).GetEndPoint(1);
			XYZ val = endPoint2 - endPoint;
			XYZ val2 = endPoint4 - endPoint3;
			XYZ val3 = new XYZ(val.Y * val2.Z - val.Z * val2.Y, val.Z * val2.X - val.X * val2.Z, val.X * val2.Y - val.Y * val2.X);
			if (val3.GetLength() < 1E-10)
			{
				return null;
			}
			bool flag = Math.Abs(val.X) < Math.Abs(val.Y);
			bool flag2 = Math.Abs(val2.X) < Math.Abs(val2.Y);
			XYZ val4;
			if (flag && !flag2)
			{
				val4 = new XYZ(endPoint.X, endPoint3.Y, (endPoint.Z + endPoint3.Z) / 2.0);
			}
			else if (!flag & flag2)
			{
				val4 = new XYZ(endPoint3.X, endPoint.Y, (endPoint.Z + endPoint3.Z) / 2.0);
			}
			else
			{
				double num = ((Math.Abs(val.X) > 1E-10 && Math.Abs(val2.Y) > 1E-10) ? ((endPoint3.X - endPoint.X + (endPoint3.Y - endPoint.Y) * val2.X / val2.Y) / (val.X - val.Y * val2.X / val2.Y)) : ((!(Math.Abs(val.Y) > 1E-10) || !(Math.Abs(val2.X) > 1E-10)) ? ((endPoint3.Z - endPoint.Z) / val.Z) : ((endPoint3.Y - endPoint.Y + (endPoint3.X - endPoint.X) * val2.Y / val2.X) / (val.Y - val.X * val2.Y / val2.X))));
				val4 = endPoint + val * num;
			}
			bool flag3 = method_2(val4, endPoint, endPoint2);
			bool flag4 = method_2(val4, endPoint3, endPoint4);
			return (val4, flag3 & flag4);
		}
		catch (Exception ex)
		{
			Logger.Error("[GetGridIntersection] 计算直线交点失败", ex);
			return null;
		}
	}

	private bool method_2(XYZ xyz_0, XYZ xyz_1, XYZ xyz_2)
	{
		XYZ val = xyz_0 - xyz_1;
		XYZ val2 = xyz_2 - xyz_1;
		double length = val2.GetLength();
		if (length < 1E-06)
		{
			return val.GetLength() < 1E-06;
		}
		double num = val.DotProduct(val2) / (length * length);
		if (num < -1E-06 || num > 1.000001)
		{
			return false;
		}
		XYZ val3 = xyz_1 + val2 * Math.Max(0.0, Math.Min(1.0, num));
		double length2 = (xyz_0 - val3).GetLength();
		return length2 < 1E-06;
	}

	private (XYZ Point, bool OnBothSegments)? method_3(Curve curve_0, Curve curve_1)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		try
		{
			double num = curve_0.GetEndParameter(0) / 2.0 + curve_0.GetEndParameter(1) / 2.0;
			XYZ val = curve_0.Evaluate(num, false);
			IntersectionResult val2 = curve_1.Project(val);
			XYZ val3 = null;
			if (val2 != null)
			{
				val3 = val2.XYZPoint;
			}
			if (val3 == null)
			{
				return null;
			}
			XYZ item = new XYZ((val.X + val3.X) / 2.0, (val.Y + val3.Y) / 2.0, (val.Z + val3.Z) / 2.0);
			return (item, false);
		}
		catch (Exception ex)
		{
			Logger.Error("[GetGridIntersection] 查找最近点失败", ex);
			return null;
		}
	}
}
