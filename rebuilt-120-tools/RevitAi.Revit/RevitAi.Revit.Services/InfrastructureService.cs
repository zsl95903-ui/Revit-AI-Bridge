using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns6;

namespace RevitAi.Revit.Services;

public sealed class InfrastructureService : IInfrastructureService
{
	private sealed class VerticalProfileSegment
	{
		public double StartStation { get; set; }

		public double EndStation { get; set; }

		public string SegmentType { get; set; } = "Line";

		public double Gradient { get; set; }

		public double StartElevation { get; set; }

		public double EndElevation { get; set; }

		public double? CurveRadius { get; set; }

		public double? CurveLength { get; set; }

		public double? TangentLength { get; set; }

		public double? ExternalDistance { get; set; }

		public VerticalCurveType? CurveType { get; set; }

		public double? ArcCenterStation { get; set; }

		public double? ArcCenterElevation { get; set; }

		public int VerticalSign { get; set; }

		public double CalculateElevation(double stationKm)
		{
			if (stationKm <= StartStation + 1E-09)
			{
				return StartElevation;
			}
			if (stationKm >= EndStation - 1E-09)
			{
				return EndElevation;
			}
			if (SegmentType == "Line")
			{
				double num = stationKm - StartStation;
				double num2 = num * 1000.0;
				return StartElevation + Gradient * num2;
			}
			if (SegmentType == "Curve")
			{
				double num3 = stationKm * 1000.0;
				double num4 = ArcCenterStation.Value * 1000.0;
				double value = ArcCenterElevation.Value;
				double value2 = CurveRadius.Value;
				double num5 = num3 - num4;
				double num6 = value2 + 1E-06;
				if (Math.Abs(num5) > num6)
				{
					return (num5 > 0.0) ? EndElevation : StartElevation;
				}
				double d = Math.Max(0.0, value2 * value2 - num5 * num5);
				double num7 = Math.Sqrt(d);
				return value - (double)VerticalSign * num7;
			}
			return StartElevation;
		}
	}

	public Result<List<RoadCenterlinePoint3D>> GenerateUnifiedSamplePoints(HorizontalCurveTable horizontalTable, VerticalCurveTable verticalTable, List<double> stationsKm)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		try
		{
			if (stationsKm == null || stationsKm.Count == 0)
			{
				LogError("桩号列表为空");
				return Result<List<RoadCenterlinePoint3D>>.Failure("桩号列表为空");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
			defaultInterpolatedStringHandler.AppendLiteral("统一采样：处理 ");
			defaultInterpolatedStringHandler.AppendFormatted(stationsKm.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个指定桩号");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			double[] item = PrepareHorizontalCurveData(horizontalTable).Azimuths;
			List<VerticalProfileSegment> verticalSegments = BuildVerticalSegments(verticalTable);
			List<RoadCenterlinePoint3D> list = new List<RoadCenterlinePoint3D>();
			foreach (double item5 in stationsKm)
			{
				(double X, double Y, double Azimuth) tuple = CalculateCoordinateAtStation(horizontalTable, item5, item);
				double item2 = tuple.X;
				double item3 = tuple.Y;
				double item4 = tuple.Azimuth;
				double z = CalculateElevationAtStation(verticalSegments, item5);
				list.Add(new RoadCenterlinePoint3D
				{
					StationKm = item5,
					X = item2,
					Y = item3,
					Z = z,
					Azimuth = item4,
					PointType = (RoadCenterlinePointType)9
				});
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("统一采样：成功生成 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个三维点（包含坐标和切线方向）");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return Result<List<RoadCenterlinePoint3D>>.Success(list);
		}
		catch (Exception ex)
		{
			LogError("生成统一采样点失败: " + ex.Message);
			return Result<List<RoadCenterlinePoint3D>>.Failure(ex.Message);
		}
	}

	public Result<RoadCenterlinePoint3D> GeneratePointAtStation(HorizontalCurveTable horizontalTable, VerticalCurveTable verticalTable, double stationKm)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		try
		{
			double[] item = PrepareHorizontalCurveData(horizontalTable).Azimuths;
			List<VerticalProfileSegment> verticalSegments = BuildVerticalSegments(verticalTable);
			(double X, double Y, double Azimuth) tuple = CalculateCoordinateAtStation(horizontalTable, stationKm, item);
			double item2 = tuple.X;
			double item3 = tuple.Y;
			double item4 = tuple.Azimuth;
			double z = CalculateElevationAtStation(verticalSegments, stationKm);
			RoadCenterlinePoint3D val = new RoadCenterlinePoint3D
			{
				StationKm = stationKm,
				X = item2,
				Y = item3,
				Z = z,
				Azimuth = item4,
				PointType = (RoadCenterlinePointType)9
			};
			return Result<RoadCenterlinePoint3D>.Success(val);
		}
		catch (Exception ex)
		{
			LogError("生成单点失败: " + ex.Message);
			return Result<RoadCenterlinePoint3D>.Failure(ex.Message);
		}
	}

	private (List<HorizontalCurveIP> IntersectionPoints, double[] Azimuths) PrepareHorizontalCurveData(HorizontalCurveTable horizontalTable)
	{
		List<HorizontalCurveIP> sortedPoints = horizontalTable.GetSortedPoints();
		if (sortedPoints.Count < 2)
		{
			throw new InvalidOperationException("平曲线表数据点不足");
		}
		double[] array = new double[sortedPoints.Count];
		for (int i = 0; i < sortedPoints.Count - 1; i++)
		{
			array[i] = CalculateAzimuth(sortedPoints[i].CoordinateX, sortedPoints[i].CoordinateY, sortedPoints[i + 1].CoordinateX, sortedPoints[i + 1].CoordinateY);
			while (array[i] < 0.0)
			{
				array[i] += Math.PI * 2.0;
			}
			while (array[i] >= Math.PI * 2.0)
			{
				array[i] -= Math.PI * 2.0;
			}
		}
		array[^1] = array[^2];
		return (IntersectionPoints: sortedPoints, Azimuths: array);
	}

	private (double X, double Y, double Azimuth) CalculateCoordinateAtStation(HorizontalCurveTable horizontalTable, double stationKm, double[] azimuths)
	{
		List<HorizontalCurveIP> sortedPoints = horizontalTable.GetSortedPoints();
		if (Math.Abs(stationKm - sortedPoints[0].Station) < 1E-09)
		{
			return (X: sortedPoints[0].CoordinateX, Y: sortedPoints[0].CoordinateY, Azimuth: azimuths[0]);
		}
		double num = sortedPoints[0].Station;
		double num2 = sortedPoints[0].CoordinateX;
		double num3 = sortedPoints[0].CoordinateY;
		int num4 = 1;
		double num6;
		int num8;
		double radius;
		double valueOrDefault2;
		double num24;
		double num25;
		double num45;
		while (true)
		{
			if (num4 < sortedPoints.Count - 1)
			{
				_ = sortedPoints[num4 - 1];
				HorizontalCurveIP val = sortedPoints[num4];
				_ = sortedPoints[num4 + 1];
				double num5 = azimuths[num4 - 1];
				num6 = azimuths[num4];
				double num7;
				for (num7 = num6 - num5; num7 > Math.PI; num7 -= Math.PI * 2.0)
				{
				}
				for (; num7 < -Math.PI; num7 += Math.PI * 2.0)
				{
				}
				num8 = ((num7 >= 0.0) ? 1 : (-1));
				double num9 = Math.Abs(num7);
				radius = val.Radius;
				double valueOrDefault = val.FirstTransitionLength.GetValueOrDefault();
				valueOrDefault2 = val.SecondTransitionLength.GetValueOrDefault();
				if (radius <= 1.0 || num9 < 1E-06)
				{
					double num10 = Math.Sqrt(Math.Pow(val.CoordinateX - num2, 2.0) + Math.Pow(val.CoordinateY - num3, 2.0));
					double num11 = num;
					if (stationKm >= num11 && stationKm <= num11 + num10 / 1000.0 + 1E-09)
					{
						double num12 = (stationKm - num11) / (num10 / 1000.0);
						double item = num2 + (val.CoordinateX - num2) * num12;
						double item2 = num3 + (val.CoordinateY - num3) * num12;
						return (X: item, Y: item2, Azimuth: num5);
					}
					num = num11 + num10 / 1000.0;
					num2 = val.CoordinateX;
					num3 = val.CoordinateY;
				}
				else
				{
					double num13 = valueOrDefault / 2.0 / radius;
					double num14 = valueOrDefault2 / 2.0 / radius;
					double num15 = ((valueOrDefault > 0.0) ? (valueOrDefault / 2.0 - Math.Pow(valueOrDefault, 3.0) / (240.0 * radius * radius)) : 0.0);
					double num16 = ((valueOrDefault > 0.0) ? (Math.Pow(valueOrDefault, 2.0) / (24.0 * radius)) : 0.0);
					double num17 = ((valueOrDefault2 > 0.0) ? (valueOrDefault2 / 2.0 - Math.Pow(valueOrDefault2, 3.0) / (240.0 * radius * radius)) : 0.0);
					double num18 = ((valueOrDefault2 > 0.0) ? (Math.Pow(valueOrDefault2, 2.0) / (24.0 * radius)) : 0.0);
					double num19 = Math.Tan(num9 / 2.0);
					double num20 = (radius + num16) * num19 + num15;
					double num21 = (radius + num18) * num19 + num17;
					double num22 = val.CoordinateX - num20 * Math.Cos(num5);
					double num23 = val.CoordinateY - num20 * Math.Sin(num5);
					num24 = val.CoordinateX + num21 * Math.Cos(num6);
					num25 = val.CoordinateY + num21 * Math.Sin(num6);
					double num26 = Math.Sqrt(Math.Pow(num22 - num2, 2.0) + Math.Pow(num23 - num3, 2.0));
					double num27 = num;
					if (stationKm >= num27 && stationKm <= num27 + num26 / 1000.0 + 1E-09)
					{
						double num28 = (stationKm - num27) / (num26 / 1000.0);
						double item3 = num2 + (num22 - num2) * num28;
						double item4 = num3 + (num23 - num3) * num28;
						return (X: item3, Y: item4, Azimuth: num5);
					}
					num = num27 + num26 / 1000.0;
					num2 = num22;
					num3 = num23;
					if (valueOrDefault > 1E-06)
					{
						double num29 = num;
						if (stationKm >= num29 && stationKm <= num29 + valueOrDefault / 1000.0 + 1E-09)
						{
							double num30 = (stationKm - num29) * 1000.0;
							double num31 = num30 - Math.Pow(num30, 5.0) / (40.0 * radius * radius * valueOrDefault * valueOrDefault);
							double num32 = Math.Pow(num30, 3.0) / (6.0 * radius * valueOrDefault);
							double item5 = num22 + num31 * Math.Cos(num5) - (double)num8 * num32 * Math.Sin(num5);
							double item6 = num23 + num31 * Math.Sin(num5) + (double)num8 * num32 * Math.Cos(num5);
							double item7 = num5 + (double)num8 * num30 * num30 / (2.0 * radius * valueOrDefault);
							return (X: item5, Y: item6, Azimuth: item7);
						}
						num = num29 + valueOrDefault / 1000.0;
					}
					double num33 = num9 - num13 - num14;
					if (num33 > 1E-06)
					{
						double num34 = num5 + (double)num8 * num13;
						double num37;
						double num38;
						if (valueOrDefault > 1E-06)
						{
							double num35 = valueOrDefault - Math.Pow(valueOrDefault, 5.0) / (40.0 * radius * radius * valueOrDefault * valueOrDefault);
							double num36 = Math.Pow(valueOrDefault, 3.0) / (6.0 * radius * valueOrDefault);
							num37 = num22 + num35 * Math.Cos(num5) - (double)num8 * num36 * Math.Sin(num5);
							num38 = num23 + num35 * Math.Sin(num5) + (double)num8 * num36 * Math.Cos(num5);
						}
						else
						{
							num37 = num22;
							num38 = num23;
						}
						double num39 = radius * num33;
						double num40 = num;
						if (stationKm >= num40 && stationKm <= num40 + num39 / 1000.0 + 1E-09)
						{
							double num41 = num37 + radius * Math.Cos(num34 + (double)num8 * Math.PI / 2.0);
							double num42 = num38 + radius * Math.Sin(num34 + (double)num8 * Math.PI / 2.0);
							double num43 = (stationKm - num40) * 1000.0;
							double num44 = num34 + (double)num8 * (num43 / radius);
							double item8 = num41 + radius * Math.Cos(num44 - (double)num8 * Math.PI / 2.0);
							double item9 = num42 + radius * Math.Sin(num44 - (double)num8 * Math.PI / 2.0);
							return (X: item8, Y: item9, Azimuth: num44);
						}
						num = num40 + num39 / 1000.0;
					}
					if (valueOrDefault2 > 1E-06)
					{
						num45 = num;
						if (stationKm >= num45 && stationKm <= num45 + valueOrDefault2 / 1000.0 + 1E-09)
						{
							break;
						}
						num = num45 + valueOrDefault2 / 1000.0;
					}
					num2 = num24;
					num3 = num25;
				}
				num4++;
				continue;
			}
			HorizontalCurveIP val2 = sortedPoints.Last();
			double num46 = Math.Sqrt(Math.Pow(val2.CoordinateX - num2, 2.0) + Math.Pow(val2.CoordinateY - num3, 2.0));
			double num47 = num;
			if (stationKm >= num47 && stationKm <= num47 + num46 / 1000.0 + 1E-09)
			{
				double num48 = (stationKm - num47) / (num46 / 1000.0);
				double item10 = num2 + (val2.CoordinateX - num2) * num48;
				double item11 = num3 + (val2.CoordinateY - num3) * num48;
				return (X: item10, Y: item11, Azimuth: azimuths[^1]);
			}
			if (stationKm <= sortedPoints[0].Station)
			{
				return (X: sortedPoints[0].CoordinateX, Y: sortedPoints[0].CoordinateY, Azimuth: azimuths[0]);
			}
			return (X: sortedPoints[sortedPoints.Count - 1].CoordinateX, Y: sortedPoints[sortedPoints.Count - 1].CoordinateY, Azimuth: azimuths[^1]);
		}
		double num49 = (stationKm - num45) * 1000.0;
		double num50 = valueOrDefault2 - num49;
		double num51 = num50 - Math.Pow(num50, 5.0) / (40.0 * radius * radius * valueOrDefault2 * valueOrDefault2);
		double num52 = Math.Pow(num50, 3.0) / (6.0 * radius * valueOrDefault2);
		double item12 = num24 - num51 * Math.Cos(num6) - (double)num8 * num52 * Math.Sin(num6);
		double item13 = num25 - num51 * Math.Sin(num6) + (double)num8 * num52 * Math.Cos(num6);
		double item14 = num6 - (double)num8 * (num50 * num50) / (2.0 * radius * valueOrDefault2);
		return (X: item12, Y: item13, Azimuth: item14);
	}

	private double CalculateElevationAtStation(List<VerticalProfileSegment> verticalSegments, double stationKm)
	{
		foreach (VerticalProfileSegment verticalSegment in verticalSegments)
		{
			if (stationKm >= verticalSegment.StartStation - 1E-09 && stationKm <= verticalSegment.EndStation + 1E-09)
			{
				return verticalSegment.CalculateElevation(stationKm);
			}
		}
		if (verticalSegments.Count > 0)
		{
			if (stationKm < verticalSegments[0].StartStation)
			{
				return verticalSegments[0].StartElevation;
			}
			if (stationKm > verticalSegments[verticalSegments.Count - 1].EndStation)
			{
				return verticalSegments[verticalSegments.Count - 1].EndElevation;
			}
		}
		return 0.0;
	}

	private List<VerticalProfileSegment> BuildVerticalSegments(VerticalCurveTable verticalCurveTable)
	{
		List<VerticalCurvePVI> sortedPoints = verticalCurveTable.GetSortedPoints();
		List<VerticalProfileSegment> list = new List<VerticalProfileSegment>();
		double num = sortedPoints[0].Station;
		double num2 = sortedPoints[0].Elevation;
		for (int i = 0; i < sortedPoints.Count; i++)
		{
			VerticalCurvePVI val = sortedPoints[i];
			if (val.CurveLength.HasValue && val.CurveLength > 0.0 && val.Radius > 0.0)
			{
				double num3 = val.FrontGradient.GetValueOrDefault() / 100.0;
				double num4 = val.BackGradient.GetValueOrDefault() / 100.0;
				double num5 = Math.Atan(num3);
				double num6 = Math.Atan(num4);
				double num7 = num4 - num3;
				bool flag;
				int num8 = ((flag = num7 > 0.0) ? 1 : (-1));
				double num9 = Math.Abs(num6 - num5);
				double num10 = val.Radius * Math.Tan(num9 / 2.0);
				double num11 = num10 * Math.Cos(num5);
				double num12 = num10 * Math.Cos(num6);
				double num13 = val.Station - num11 / 1000.0;
				double num14 = val.Elevation - num3 * num11;
				double num15 = val.Station + num12 / 1000.0;
				double num16 = val.Elevation + num4 * num12;
				if (num13 > num + 1E-09)
				{
					double num17 = (num13 - num) * 1000.0;
					list.Add(new VerticalProfileSegment
					{
						StartStation = num,
						EndStation = num13,
						StartElevation = num2,
						EndElevation = num14,
						SegmentType = "Line",
						Gradient = (num14 - num2) / num17
					});
				}
				double num18 = (double)(-num8) * val.Radius * Math.Sin(num5);
				double num19 = (double)num8 * val.Radius * Math.Cos(num5);
				double num20 = num13 * 1000.0 + num18;
				double value = num14 + num19;
				list.Add(new VerticalProfileSegment
				{
					StartStation = num13,
					EndStation = num15,
					StartElevation = num14,
					EndElevation = num16,
					SegmentType = "Curve",
					CurveRadius = val.Radius,
					CurveType = (VerticalCurveType)(flag ? 1 : 0),
					ArcCenterStation = num20 / 1000.0,
					ArcCenterElevation = value,
					VerticalSign = num8
				});
				num = num15;
				num2 = num16;
			}
			else if (i == sortedPoints.Count - 1 && val.Station > num + 1E-09)
			{
				double num21 = (val.Station - num) * 1000.0;
				list.Add(new VerticalProfileSegment
				{
					StartStation = num,
					EndStation = val.Station,
					StartElevation = num2,
					EndElevation = val.Elevation,
					SegmentType = "Line",
					Gradient = (val.Elevation - num2) / num21
				});
			}
		}
		list = list.OrderBy((VerticalProfileSegment s) => s.StartStation).ToList();
		for (int num22 = 0; num22 < list.Count - 1; num22++)
		{
			if (list[num22].EndStation > list[num22 + 1].StartStation + 1E-09)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 4);
				defaultInterpolatedStringHandler.AppendLiteral("⚠️ 警告: 竖曲线段 ");
				defaultInterpolatedStringHandler.AppendFormatted(num22);
				defaultInterpolatedStringHandler.AppendLiteral(" 与 ");
				defaultInterpolatedStringHandler.AppendFormatted(num22 + 1);
				defaultInterpolatedStringHandler.AppendLiteral(" 发生重叠（Station ");
				defaultInterpolatedStringHandler.AppendFormatted(list[num22].EndStation, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(" > ");
				defaultInterpolatedStringHandler.AppendFormatted(list[num22 + 1].StartStation, "F3");
				defaultInterpolatedStringHandler.AppendLiteral("），已强制衔接");
				LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
				list[num22].EndStation = list[num22 + 1].StartStation;
				list[num22].EndElevation = list[num22 + 1].StartElevation;
			}
		}
		return list;
	}

	private static double CalculateAzimuth(double x1, double y1, double x2, double y2)
	{
		double x3 = x2 - x1;
		double y3 = y2 - y1;
		double num;
		for (num = Math.Atan2(y3, x3); num < 0.0; num += Math.PI * 2.0)
		{
		}
		while (num >= Math.PI * 2.0)
		{
			num -= Math.PI * 2.0;
		}
		return num;
	}

	private static void LogError(string message)
	{
		Logger.Error(message);
	}

	private static void LogInfo(string message)
	{
		Logger.Info(message);
	}

	private static void LogWarning(string message)
	{
		Logger.Warning(message);
	}
}
