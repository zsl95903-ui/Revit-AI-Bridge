using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ns6;

namespace RevitAi.Revit.Services;

public sealed class ExcelDataService : IExcelDataService
{
	public Result<List<string>> GetSheetNames(string filePath)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		try
		{
			if (File.Exists(filePath))
			{
				ExcelPackage.LicenseContext = (LicenseContext)0;
				ExcelPackage val = new ExcelPackage(new FileInfo(filePath));
				try
				{
					List<string> list = new List<string>();
					foreach (ExcelWorksheet worksheet in val.Workbook.Worksheets)
					{
						list.Add(worksheet.Name);
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
					defaultInterpolatedStringHandler.AppendLiteral("成功获取工作表名称列表，共 ");
					defaultInterpolatedStringHandler.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个工作表");
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
					return Result<List<string>>.Success(list);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			LogError("文件不存在: " + filePath);
			return Result<List<string>>.Failure("文件不存在: " + filePath);
		}
		catch (Exception ex)
		{
			LogError("获取工作表名称失败: " + ex.Message, ex);
			return Result<List<string>>.Failure("获取工作表名称失败: " + ex.Message);
		}
	}

	public Result<StationCoordinateTable> ReadStationCoordinateTable(string filePath, string sheetName, StationCoordinateReadConfig? config = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		try
		{
			if (config == null)
			{
				config = new StationCoordinateReadConfig();
			}
			ExcelPackage.LicenseContext = (LicenseContext)0;
			ExcelPackage val = new ExcelPackage(new FileInfo(filePath));
			try
			{
				ExcelWorksheet val2 = val.Workbook.Worksheets[sheetName];
				if (val2 == null)
				{
					LogError("工作表 '" + sheetName + "' 不存在");
					return Result<StationCoordinateTable>.Failure("工作表 '" + sheetName + "' 不存在");
				}
				StationCoordinateTable val3 = new StationCoordinateTable
				{
					Source = filePath
				};
				int rows = val2.Dimension.Rows;
				List<string> list = new List<string>();
				StationCoordinate val4 = default(StationCoordinate);
				for (int i = config.DataStartRow; i <= rows; i++)
				{
					try
					{
						string text = ((ExcelRangeBase)val2.Cells[i, config.StationColumn]).Text;
						if (string.IsNullOrWhiteSpace(text))
						{
							continue;
						}
						if (!StationCoordinate.TryParse(text, out val4))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
							defaultInterpolatedStringHandler.AppendLiteral("第");
							defaultInterpolatedStringHandler.AppendFormatted(i);
							defaultInterpolatedStringHandler.AppendLiteral("行: 无法解析桩号 '");
							defaultInterpolatedStringHandler.AppendFormatted(text);
							defaultInterpolatedStringHandler.AppendLiteral("'");
							list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
							continue;
						}
						val4.CoordinateX = ParseDouble(((ExcelRangeBase)val2.Cells[i, config.XCoordinateColumn]).Text);
						val4.CoordinateY = ParseDouble(((ExcelRangeBase)val2.Cells[i, config.YCoordinateColumn]).Text);
						if (config.ElevationColumn.HasValue)
						{
							string text2 = ((ExcelRangeBase)val2.Cells[i, config.ElevationColumn.Value]).Text;
							if (!string.IsNullOrWhiteSpace(text2))
							{
								val4.Elevation = ParseDouble(text2);
							}
						}
						if (config.AzimuthColumn.HasValue)
						{
							string text3 = ((ExcelRangeBase)val2.Cells[i, config.AzimuthColumn.Value]).Text;
							if (!string.IsNullOrWhiteSpace(text3))
							{
								val4.Azimuth = ParseDouble(text3);
							}
						}
						val3.Coordinates.Add(val4);
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("第");
						defaultInterpolatedStringHandler2.AppendFormatted(i);
						defaultInterpolatedStringHandler2.AppendLiteral("行: ");
						defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
				}
				if (val3.Coordinates.Count == 0)
				{
					LogError("未读取到有效的逐桩坐标数据");
					return Result<StationCoordinateTable>.Failure("未读取到有效的逐桩坐标数据");
				}
				if (list.Count > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("读取逐桩坐标表时发生");
					defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler3.AppendLiteral("个错误: ");
					defaultInterpolatedStringHandler3.AppendFormatted(string.Join("; ", list));
					LogWarning(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("成功读取");
				defaultInterpolatedStringHandler4.AppendFormatted(val3.Coordinates.Count);
				defaultInterpolatedStringHandler4.AppendLiteral("个逐桩坐标点");
				LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
				return Result<StationCoordinateTable>.Success(val3);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			LogError("读取逐桩坐标表失败: " + ex2.Message);
			return Result<StationCoordinateTable>.Failure(ex2.Message);
		}
	}

	public Result<HorizontalCurveTable> ReadHorizontalCurveTable(string filePath, string sheetName, HorizontalCurveReadConfig? config = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		try
		{
			if (config == null)
			{
				config = new HorizontalCurveReadConfig();
			}
			ExcelPackage.LicenseContext = (LicenseContext)0;
			ExcelPackage val = new ExcelPackage(new FileInfo(filePath));
			try
			{
				ExcelWorksheet val2 = val.Workbook.Worksheets[sheetName];
				if (val2 == null)
				{
					LogError("工作表 '" + sheetName + "' 不存在");
					return Result<HorizontalCurveTable>.Failure("工作表 '" + sheetName + "' 不存在");
				}
				HorizontalCurveTable val3 = new HorizontalCurveTable
				{
					Source = filePath
				};
				int rows = val2.Dimension.Rows;
				List<string> list = new List<string>();
				for (int i = config.DataStartRow; i <= rows; i++)
				{
					try
					{
						HorizontalCurveIP val4 = new HorizontalCurveIP
						{
							IPNumber = ((ExcelRangeBase)val2.Cells[i, config.IPNumberColumn]).Text,
							Station = ParseStationKmSafe(((ExcelRangeBase)val2.Cells[i, config.StationColumn]).Text),
							CoordinateX = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.XCoordinateColumn]).Text),
							CoordinateY = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.YCoordinateColumn]).Text),
							Radius = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.RadiusColumn]).Text)
						};
						if (config.FirstTransitionLengthColumn.HasValue)
						{
							val4.FirstTransitionLength = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.FirstTransitionLengthColumn.Value]).Text);
						}
						if (config.SecondTransitionLengthColumn.HasValue)
						{
							val4.SecondTransitionLength = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.SecondTransitionLengthColumn.Value]).Text);
						}
						val3.IntersectionPoints.Add(val4);
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler.AppendLiteral("第");
						defaultInterpolatedStringHandler.AppendFormatted(i);
						defaultInterpolatedStringHandler.AppendLiteral("行: ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				if (list.Count > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(14, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("读取平曲线表时发生");
					defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler2.AppendLiteral("个错误: ");
					defaultInterpolatedStringHandler2.AppendFormatted(string.Join("; ", list));
					LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("成功读取");
				defaultInterpolatedStringHandler3.AppendFormatted(val3.IntersectionPoints.Count);
				defaultInterpolatedStringHandler3.AppendLiteral("个交点数据");
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
				return Result<HorizontalCurveTable>.Success(val3);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			LogError("读取平曲线表失败: " + ex2.Message);
			return Result<HorizontalCurveTable>.Failure(ex2.Message);
		}
	}

	public Result<StationElevationTable> ReadStationElevationTable(string filePath, string sheetName, StationElevationReadConfig? config = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		try
		{
			if (config == null)
			{
				config = new StationElevationReadConfig();
			}
			ExcelPackage.LicenseContext = (LicenseContext)0;
			ExcelPackage val = new ExcelPackage(new FileInfo(filePath));
			try
			{
				ExcelWorksheet val2 = val.Workbook.Worksheets[sheetName];
				if (val2 == null)
				{
					LogError("工作表 '" + sheetName + "' 不存在");
					return Result<StationElevationTable>.Failure("工作表 '" + sheetName + "' 不存在");
				}
				StationElevationTable val3 = new StationElevationTable
				{
					Source = filePath
				};
				int rows = val2.Dimension.Rows;
				List<string> list = new List<string>();
				StationCoordinate val4 = default(StationCoordinate);
				for (int i = config.DataStartRow; i <= rows; i++)
				{
					try
					{
						string text = ((ExcelRangeBase)val2.Cells[i, config.StationColumn]).Text;
						if (string.IsNullOrWhiteSpace(text))
						{
							continue;
						}
						if (!StationCoordinate.TryParse(text, out val4))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
							defaultInterpolatedStringHandler.AppendLiteral("第");
							defaultInterpolatedStringHandler.AppendFormatted(i);
							defaultInterpolatedStringHandler.AppendLiteral("行: 无法解析桩号 '");
							defaultInterpolatedStringHandler.AppendFormatted(text);
							defaultInterpolatedStringHandler.AppendLiteral("'");
							list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
							continue;
						}
						StationElevation val5 = new StationElevation
						{
							Prefix = val4.Prefix,
							StationInteger = val4.StationInteger,
							StationDecimal = val4.StationDecimal,
							Elevation = ParseDouble(((ExcelRangeBase)val2.Cells[i, config.ElevationColumn]).Text)
						};
						if (config.GradientColumn.HasValue)
						{
							string text2 = ((ExcelRangeBase)val2.Cells[i, config.GradientColumn.Value]).Text;
							if (!string.IsNullOrWhiteSpace(text2))
							{
								val5.Gradient = ParseDouble(text2);
							}
						}
						val3.Elevations.Add(val5);
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("第");
						defaultInterpolatedStringHandler2.AppendFormatted(i);
						defaultInterpolatedStringHandler2.AppendLiteral("行: ");
						defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
				}
				if (val3.Elevations.Count == 0)
				{
					LogError("未读取到有效的桩号高程数据");
					return Result<StationElevationTable>.Failure("未读取到有效的桩号高程数据");
				}
				if (list.Count > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(15, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("读取桩号高程表时发生");
					defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler3.AppendLiteral("个错误: ");
					defaultInterpolatedStringHandler3.AppendFormatted(string.Join("; ", list));
					LogWarning(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(8, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("成功读取");
				defaultInterpolatedStringHandler4.AppendFormatted(val3.Elevations.Count);
				defaultInterpolatedStringHandler4.AppendLiteral("个高程点");
				LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
				return Result<StationElevationTable>.Success(val3);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			LogError("读取桩号高程表失败: " + ex2.Message);
			return Result<StationElevationTable>.Failure(ex2.Message);
		}
	}

	public Result<VerticalCurveTable> ReadVerticalCurveTable(string filePath, string sheetName, VerticalCurveReadConfig? config = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		try
		{
			if (config == null)
			{
				config = new VerticalCurveReadConfig();
			}
			ExcelPackage.LicenseContext = (LicenseContext)0;
			ExcelPackage val = new ExcelPackage(new FileInfo(filePath));
			try
			{
				ExcelWorksheet val2 = val.Workbook.Worksheets[sheetName];
				if (val2 == null)
				{
					LogError("工作表 '" + sheetName + "' 不存在");
					return Result<VerticalCurveTable>.Failure("工作表 '" + sheetName + "' 不存在");
				}
				VerticalCurveTable val3 = new VerticalCurveTable
				{
					Source = filePath
				};
				int rows = val2.Dimension.Rows;
				List<string> list = new List<string>();
				for (int i = config.DataStartRow; i <= rows; i++)
				{
					try
					{
						double num = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.RadiusColumn]).Text);
						VerticalCurveType val4 = ((!(num < 0.0)) ? VerticalCurveType.Concave : VerticalCurveType.Convex);
						string text = ((ExcelRangeBase)val2.Cells[i, config.CurveTypeColumn]).Text;
						if (!string.IsNullOrWhiteSpace(text))
						{
							if (Enum.TryParse<VerticalCurveType>(text, out VerticalCurveType result))
							{
								val4 = result;
							}
							else if (text.Contains("凸"))
							{
								val4 = (VerticalCurveType)0;
							}
							else if (text.Contains("凹"))
							{
								val4 = (VerticalCurveType)1;
							}
						}
						double radius = (((int)val4 == 0) ? (0.0 - Math.Abs(num)) : Math.Abs(num));
						VerticalCurvePVI val5 = new VerticalCurvePVI
						{
							PointNumber = (config.PointNumberColumn.HasValue ? ((ExcelRangeBase)val2.Cells[i, config.PointNumberColumn.Value]).Text : string.Empty),
							Station = ParseStationKmSafe(((ExcelRangeBase)val2.Cells[i, config.StationColumn]).Text),
							Elevation = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.ElevationColumn]).Text),
							Radius = radius,
							CurveType = val4
						};
						if (config.FrontGradientColumn.HasValue)
						{
							val5.FrontGradient = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.FrontGradientColumn.Value]).Text);
						}
						if (config.BackGradientColumn.HasValue)
						{
							val5.BackGradient = ParseDoubleSafe(((ExcelRangeBase)val2.Cells[i, config.BackGradientColumn.Value]).Text);
						}
						val3.Points.Add(val5);
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler.AppendLiteral("第");
						defaultInterpolatedStringHandler.AppendFormatted(i);
						defaultInterpolatedStringHandler.AppendLiteral("行: ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				if (list.Count > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(14, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("读取竖曲线表时发生");
					defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler2.AppendLiteral("个错误: ");
					defaultInterpolatedStringHandler2.AppendFormatted(string.Join("; ", list));
					LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("成功读取");
				defaultInterpolatedStringHandler3.AppendFormatted(val3.Points.Count);
				defaultInterpolatedStringHandler3.AppendLiteral("个竖曲线点");
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
				return Result<VerticalCurveTable>.Success(val3);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			LogError("读取竖曲线表失败: " + ex2.Message);
			return Result<VerticalCurveTable>.Failure(ex2.Message);
		}
	}

	private static double ParseStationKmSafe(string stationValue)
	{
		StationCoordinate val = default(StationCoordinate);
		if (StationCoordinate.TryParse(stationValue, out val))
		{
			return val.FullStationKm;
		}
		if (double.TryParse(stationValue, out var result))
		{
			return result;
		}
		return 0.0;
	}

	private static double ParseStationKm(string stationValue)
	{
		StationCoordinate val = default(StationCoordinate);
		if (StationCoordinate.TryParse(stationValue, out val))
		{
			return val.FullStationKm;
		}
		if (double.TryParse(stationValue, out var result))
		{
			return result;
		}
		throw new FormatException("无法解析桩号: " + stationValue);
	}

	private static double ParseDoubleSafe(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return 0.0;
		}
		value = value.Trim();
		if (double.TryParse(value, out var result))
		{
			return result;
		}
		return 0.0;
	}

	private static double ParseDouble(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return 0.0;
		}
		value = value.Trim();
		if (double.TryParse(value, out var result))
		{
			return result;
		}
		throw new FormatException("无法解析数值: " + value);
	}

	private static void LogError(string message)
	{
	}

	private static void LogError(string message, Exception? ex)
	{
		if (ex != null)
		{
		}
	}

	private static void LogInfo(string message)
	{
	}

	private static void LogWarning(string message)
	{
	}

	public Result<Dictionary<string, ManholeParameterData>> ReadManholeParameterTable(string filePath, string sheetName, ManholeParameterReadConfig? config = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		try
		{
			if (config == null)
			{
				config = new ManholeParameterReadConfig();
			}
			ExcelPackage.LicenseContext = (LicenseContext)0;
			ExcelPackage val = new ExcelPackage(new FileInfo(filePath));
			try
			{
				ExcelWorksheet val2 = val.Workbook.Worksheets[sheetName];
				if (val2 == null)
				{
					LogError("工作表 '" + sheetName + "' 不存在");
					return Result<Dictionary<string, ManholeParameterData>>.Failure("工作表 '" + sheetName + "' 不存在");
				}
				Dictionary<string, ManholeParameterData> dictionary = new Dictionary<string, ManholeParameterData>();
				int rows = val2.Dimension.Rows;
				List<string> list = new List<string>();
				for (int i = config.DataStartRow; i <= rows; i++)
				{
					try
					{
						string text = ((ExcelRangeBase)val2.Cells[i, config.ManholeIdColumn]).Text;
						if (string.IsNullOrWhiteSpace(text))
						{
							continue;
						}
						ManholeParameterData val3 = new ManholeParameterData
						{
							ManholeId = text.Trim(),
							GroundElevationM = ParseDouble(((ExcelRangeBase)val2.Cells[i, config.GroundElevationColumn]).Text),
							PipeBottomElevationM = ParseDouble(((ExcelRangeBase)val2.Cells[i, config.PipeBottomElevationColumn]).Text),
							WellDepthM = ParseDouble(((ExcelRangeBase)val2.Cells[i, config.WellDepthColumn]).Text),
							Source = filePath
						};
						if (!val3.IsValid())
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
							defaultInterpolatedStringHandler.AppendLiteral("第");
							defaultInterpolatedStringHandler.AppendFormatted(i);
							defaultInterpolatedStringHandler.AppendLiteral("行: 井编号无效或为空");
							list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
							continue;
						}
						if (dictionary.ContainsKey(val3.ManholeId))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(21, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("第");
							defaultInterpolatedStringHandler2.AppendFormatted(i);
							defaultInterpolatedStringHandler2.AppendLiteral("行: 井编号 '");
							defaultInterpolatedStringHandler2.AppendFormatted(val3.ManholeId);
							defaultInterpolatedStringHandler2.AppendLiteral("' 重复，将覆盖原有数据");
							LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
						dictionary[val3.ManholeId] = val3;
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("第");
						defaultInterpolatedStringHandler3.AppendFormatted(i);
						defaultInterpolatedStringHandler3.AppendLiteral("行: ");
						defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
					}
				}
				if (dictionary.Count == 0)
				{
					LogError("未读取到有效的管井参数数据");
					return Result<Dictionary<string, ManholeParameterData>>.Failure("未读取到有效的管井参数数据");
				}
				if (list.Count > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(15, 2);
					defaultInterpolatedStringHandler4.AppendLiteral("读取管井参数表时发生");
					defaultInterpolatedStringHandler4.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler4.AppendLiteral("个错误: ");
					defaultInterpolatedStringHandler4.AppendFormatted(string.Join("; ", list));
					LogWarning(defaultInterpolatedStringHandler4.ToStringAndClear());
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler5.AppendLiteral("成功读取");
				defaultInterpolatedStringHandler5.AppendFormatted(dictionary.Count);
				defaultInterpolatedStringHandler5.AppendLiteral("个管井参数");
				LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
				return Result<Dictionary<string, ManholeParameterData>>.Success(dictionary);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			LogError("读取管井参数表失败: " + ex2.Message);
			return Result<Dictionary<string, ManholeParameterData>>.Failure(ex2.Message);
		}
	}

	public Result<PilePositionExcelData> ReadPilePositionTable(string filePath, string sheetName, PilePositionReadConfig? config = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Expected O, but got Unknown
		try
		{
			if (config == null)
			{
				config = new PilePositionReadConfig();
			}
			ExcelPackage.LicenseContext = (LicenseContext)0;
			ExcelPackage val = new ExcelPackage(new FileInfo(filePath));
			try
			{
				ExcelWorksheet val2 = val.Workbook.Worksheets[sheetName];
				if (val2 == null)
				{
					LogError("工作表 '" + sheetName + "' 不存在");
					return Result<PilePositionExcelData>.Failure("工作表 '" + sheetName + "' 不存在");
				}
				PilePositionExcelData val3 = new PilePositionExcelData();
				int rows = val2.Dimension.Rows;
				List<string> list = new List<string>();
				for (int i = config.DataStartRow; i <= rows; i++)
				{
					try
					{
						string text = ((ExcelRangeBase)val2.Cells[i, config.PierNumberColumn]).Text;
						if (string.IsNullOrWhiteSpace(text))
						{
							continue;
						}
						string text2 = ((ExcelRangeBase)val2.Cells[i, config.PileNumberColumn]).Text;
						if (string.IsNullOrWhiteSpace(text2))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
							defaultInterpolatedStringHandler.AppendLiteral("第");
							defaultInterpolatedStringHandler.AppendFormatted(i);
							defaultInterpolatedStringHandler.AppendLiteral("行: 桩基编号不能为空");
							list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
							continue;
						}
						string text3 = ((ExcelRangeBase)val2.Cells[i, config.XCoordinateColumn]).Text;
						if (!double.TryParse(text3, out var result))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(12, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("第");
							defaultInterpolatedStringHandler2.AppendFormatted(i);
							defaultInterpolatedStringHandler2.AppendLiteral("行: X坐标无效 '");
							defaultInterpolatedStringHandler2.AppendFormatted(text3);
							defaultInterpolatedStringHandler2.AppendLiteral("'");
							list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
							continue;
						}
						string text4 = ((ExcelRangeBase)val2.Cells[i, config.YCoordinateColumn]).Text;
						if (!double.TryParse(text4, out var result2))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(12, 2);
							defaultInterpolatedStringHandler3.AppendLiteral("第");
							defaultInterpolatedStringHandler3.AppendFormatted(i);
							defaultInterpolatedStringHandler3.AppendLiteral("行: Y坐标无效 '");
							defaultInterpolatedStringHandler3.AppendFormatted(text4);
							defaultInterpolatedStringHandler3.AppendLiteral("'");
							list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
							continue;
						}
						double? pileLength = null;
						if (config.PileLengthColumn.HasValue)
						{
							string text5 = ((ExcelRangeBase)val2.Cells[i, config.PileLengthColumn.Value]).Text;
							if (!string.IsNullOrWhiteSpace(text5))
							{
								if (double.TryParse(text5, out var result3))
								{
									pileLength = result3;
								}
								else
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(15, 2);
									defaultInterpolatedStringHandler4.AppendLiteral("第");
									defaultInterpolatedStringHandler4.AppendFormatted(i);
									defaultInterpolatedStringHandler4.AppendLiteral("行: 桩长无效 '");
									defaultInterpolatedStringHandler4.AppendFormatted(text5);
									defaultInterpolatedStringHandler4.AppendLiteral("'，已忽略");
									list.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
								}
							}
						}
						PilePositionExcelItem item = new PilePositionExcelItem
						{
							PierNumber = text.Trim(),
							PileNumber = text2.Trim(),
							XCoordinate = result,
							YCoordinate = result2,
							PileLength = pileLength
						};
						val3.Positions.Add(item);
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler5.AppendLiteral("第");
						defaultInterpolatedStringHandler5.AppendFormatted(i);
						defaultInterpolatedStringHandler5.AppendLiteral("行: ");
						defaultInterpolatedStringHandler5.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler5.ToStringAndClear());
					}
				}
				if (val3.Positions.Count == 0)
				{
					LogError("未读取到有效的桩定位数据");
					return Result<PilePositionExcelData>.Failure("未读取到有效的桩定位数据");
				}
				if (list.Count > 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(14, 2);
					defaultInterpolatedStringHandler6.AppendLiteral("读取桩定位表时发生");
					defaultInterpolatedStringHandler6.AppendFormatted(list.Count);
					defaultInterpolatedStringHandler6.AppendLiteral("个错误: ");
					defaultInterpolatedStringHandler6.AppendFormatted(string.Join("; ", list));
					LogWarning(defaultInterpolatedStringHandler6.ToStringAndClear());
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler7.AppendLiteral("成功读取");
				defaultInterpolatedStringHandler7.AppendFormatted(val3.Positions.Count);
				defaultInterpolatedStringHandler7.AppendLiteral("个桩定位数据");
				LogInfo(defaultInterpolatedStringHandler7.ToStringAndClear());
				return Result<PilePositionExcelData>.Success(val3);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			LogError("读取桩定位表失败: " + ex2.Message);
			return Result<PilePositionExcelData>.Failure(ex2.Message);
		}
	}

	public Result<string> ExportDataToExcel(string? filePath, string sheetName, List<List<string>> data, bool autoFormat = true)
	{
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		try
		{
			if (data == null || data.Count == 0)
			{
				LogError("导出数据不能为空");
				return Result<string>.Failure("导出数据不能为空");
			}
			if (string.IsNullOrEmpty(filePath))
			{
				SaveFileDialog obj = new SaveFileDialog
				{
					Filter = "Excel 文件|*.xlsx|所有文件|*.*",
					DefaultExt = "xlsx"
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
				defaultInterpolatedStringHandler.AppendFormatted(sheetName);
				defaultInterpolatedStringHandler.AppendLiteral("_");
				defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now, "yyyyMMdd_HHmmss");
				defaultInterpolatedStringHandler.AppendLiteral(".xlsx");
				obj.FileName = defaultInterpolatedStringHandler.ToStringAndClear();
				SaveFileDialog saveFileDialog = obj;
				if (saveFileDialog.ShowDialog() != true)
				{
					LogInfo("用户取消了保存操作");
					return Result<string>.Failure("用户取消了保存操作");
				}
				filePath = saveFileDialog.FileName;
			}
			string text = filePath;
			ExcelPackage.LicenseContext = (LicenseContext)0;
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			ExcelPackage val = new ExcelPackage(new FileInfo(text));
			try
			{
				ExcelWorksheet val2 = val.Workbook.Worksheets.Add(sheetName);
				for (int i = 0; i < data.Count; i++)
				{
					List<string> list = data[i];
					for (int j = 0; j < list.Count; j++)
					{
						((ExcelRangeBase)val2.Cells[i + 1, j + 1]).Value = list[j];
					}
				}
				if (autoFormat)
				{
					ExcelRange val3 = val2.Cells[1, 1, 1, data[0].Count];
					((ExcelRangeBase)val3).Style.Font.Bold = true;
					((ExcelRangeBase)val3).Style.Fill.PatternType = (ExcelFillStyle)1;
					((ExcelRangeBase)val3).Style.Fill.BackgroundColor.SetColor(Color.LightGray);
					((ExcelRangeBase)val3).Style.HorizontalAlignment = (ExcelHorizontalAlignment)2;
					((ExcelRangeBase)val2.Cells[val2.Dimension.Address]).AutoFitColumns();
					for (int k = 1; k <= data[0].Count; k++)
					{
						if (val2.Column(k).Width < 10.0)
						{
							val2.Column(k).Width = 10.0;
						}
					}
				}
				val.Save();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("成功导出数据到 Excel 文件: ");
				defaultInterpolatedStringHandler2.AppendFormatted(text);
				defaultInterpolatedStringHandler2.AppendLiteral("，共 ");
				defaultInterpolatedStringHandler2.AppendFormatted(data.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 行");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				return Result<string>.Success(text);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			LogError("导出 Excel 失败: " + ex.Message);
			return Result<string>.Failure("导出 Excel 失败: " + ex.Message);
		}
	}
}
