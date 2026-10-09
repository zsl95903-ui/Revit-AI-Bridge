using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Models;

public class StairCreationParameters
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private double double_0 = 300.0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double double_1 = 0.0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double double_2 = 3000.0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private IList<int> ilist_0 = new List<int> { 12, 12, 12 };

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double double_3 = 1200.0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double double_4 = 200.0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double double_5 = 1200.0;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private StairDirection stairDirection_0 = StairDirection.Left;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private XYZ xyz_0 = XYZ.BasisX;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private XYZ xyz_1 = XYZ.Zero;

	public double TreadDepthMm
	{
		[CompilerGenerated]
		get
		{
			return double_0;
		}
		[CompilerGenerated]
		set
		{
			double_0 = value;
		}
	}

	public double BaseElevationM
	{
		[CompilerGenerated]
		get
		{
			return double_1;
		}
		[CompilerGenerated]
		set
		{
			double_1 = value;
		}
	}

	public double TotalHeightMm
	{
		[CompilerGenerated]
		get
		{
			return double_2;
		}
		[CompilerGenerated]
		set
		{
			double_2 = value;
		}
	}

	public IList<int> StepsPerRun
	{
		[CompilerGenerated]
		get
		{
			return ilist_0;
		}
		[CompilerGenerated]
		set
		{
			ilist_0 = value;
		}
	}

	public double RunWidthMm
	{
		[CompilerGenerated]
		get
		{
			return double_3;
		}
		[CompilerGenerated]
		set
		{
			double_3 = value;
		}
	}

	public double WellWidthMm
	{
		[CompilerGenerated]
		get
		{
			return double_4;
		}
		[CompilerGenerated]
		set
		{
			double_4 = value;
		}
	}

	public double LandingWidthMm
	{
		[CompilerGenerated]
		get
		{
			return double_5;
		}
		[CompilerGenerated]
		set
		{
			double_5 = value;
		}
	}

	public StairDirection Direction
	{
		[CompilerGenerated]
		get
		{
			return stairDirection_0;
		}
		[CompilerGenerated]
		set
		{
			stairDirection_0 = value;
		}
	}

	public XYZ StartDirection
	{
		[CompilerGenerated]
		get
		{
			return xyz_0;
		}
		[CompilerGenerated]
		set
		{
			xyz_0 = value;
		}
	}

	public XYZ InsertPoint
	{
		[CompilerGenerated]
		get
		{
			return xyz_1;
		}
		[CompilerGenerated]
		set
		{
			xyz_1 = value;
		}
	}

	public double RiserHeightMm => TotalHeightMm / (double)TotalSteps;

	public int TotalSteps => (StepsPerRun.Count > 0) ? StepsPerRun.Sum() : 0;

	public static IList<int> AutoDistributeSteps(double totalHeightMm, int runCount)
	{
		double num = 150.0;
		int num2 = (int)Math.Round(totalHeightMm / num);
		double num3 = totalHeightMm / (double)num2;
		if (num3 < 150.0)
		{
			num2 = (int)Math.Ceiling(totalHeightMm / 150.0);
		}
		else if (num3 > 220.0)
		{
			num2 = (int)Math.Floor(totalHeightMm / 220.0);
		}
		List<int> list = new List<int>();
		int num4 = num2 / runCount;
		int num5 = num2 % runCount;
		for (int i = 0; i < runCount; i++)
		{
			int item = num4 + ((i < num5) ? 1 : 0);
			list.Add(item);
		}
		return list;
	}

	public static StairCreationParameters CreateAuto(double totalHeightMm, int runCount, double treadDepthMm = 300.0)
	{
		IList<int> stepsPerRun = AutoDistributeSteps(totalHeightMm, runCount);
		return new StairCreationParameters
		{
			TotalHeightMm = totalHeightMm,
			TreadDepthMm = treadDepthMm,
			StepsPerRun = stepsPerRun,
			BaseElevationM = 0.0,
			RunWidthMm = 1200.0,
			WellWidthMm = 200.0,
			LandingWidthMm = 1200.0,
			Direction = StairDirection.Left,
			StartDirection = XYZ.BasisX
		};
	}

	public bool Validate(out string errorMessage)
	{
		errorMessage = string.Empty;
		if (TreadDepthMm <= 0.0)
		{
			errorMessage = "踏步宽度必须大于0";
			return false;
		}
		if (TotalHeightMm <= 0.0)
		{
			errorMessage = "总高度必须大于0";
			return false;
		}
		if (StepsPerRun == null || StepsPerRun.Count == 0)
		{
			errorMessage = "必须至少指定一跑的踏步数";
			return false;
		}
		if (StepsPerRun.Any((int int_0) => int_0 <= 0))
		{
			errorMessage = "每跑的踏步数必须大于0";
			return false;
		}
		if (RunWidthMm <= 0.0)
		{
			errorMessage = "梯段宽度必须大于0";
			return false;
		}
		if (WellWidthMm < 0.0)
		{
			errorMessage = "梯井宽度不能为负数";
			return false;
		}
		if (LandingWidthMm <= 0.0)
		{
			errorMessage = "平台宽度必须大于0";
			return false;
		}
		double riserHeightMm = RiserHeightMm;
		if (riserHeightMm < 150.0 || riserHeightMm > 220.0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
			defaultInterpolatedStringHandler.AppendLiteral("计算出的踏步高度 ");
			defaultInterpolatedStringHandler.AppendFormatted(riserHeightMm, "F1");
			defaultInterpolatedStringHandler.AppendLiteral("mm 超出合理范围（150-220mm），请调整总高度或踏步数");
			errorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
			return false;
		}
		return true;
	}

	public string GetSummary()
	{
		string text = string.Join(" + ", StepsPerRun.Select(delegate(int int_0, int int_1)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
			defaultInterpolatedStringHandler.AppendLiteral("第");
			defaultInterpolatedStringHandler.AppendFormatted(int_1 + 1);
			defaultInterpolatedStringHandler.AppendLiteral("跑:");
			defaultInterpolatedStringHandler.AppendFormatted(int_0);
			defaultInterpolatedStringHandler.AppendLiteral("步");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}));
		string text2 = smethod_0(StartDirection);
		string format = "楼梯参数:\n  踏步宽度: {0:F0} mm\n  楼梯底标高: {1:F2} m\n  总高度: {2:F0} mm\n  总踏步数: {3} ({4})\n  踏步高度: {5:F1} mm\n  梯段宽度: {6:F0} mm\n  梯井宽度: {7:F0} mm\n  平台宽度: {8:F0} mm\n  起始方向: {9}\n  上楼方向: {10}";
		InlineArray11<object> gparam_ = default(InlineArray11<object>);
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 0) = TreadDepthMm;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 1) = BaseElevationM;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 2) = TotalHeightMm;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 3) = TotalSteps;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 4) = text;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 5) = RiserHeightMm;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 6) = RunWidthMm;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 7) = WellWidthMm;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 8) = LandingWidthMm;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 9) = text2;
		Class653.smethod_2<InlineArray11<object>, object>(ref gparam_, 10) = Direction;
		return string.Format(format, Class653.smethod_1<InlineArray11<object>, object>(in gparam_, 11));
	}

	private static string smethod_0(XYZ xyz_2)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (xyz_2 == null)
		{
			return "未知";
		}
		XYZ val = new XYZ(xyz_2.X, xyz_2.Y, 0.0);
		if (val.GetLength() == 0.0)
		{
			return "未知";
		}
		if (val.X > 0.9)
		{
			return "X正方向";
		}
		if (val.X < -0.9)
		{
			return "X负方向";
		}
		if (val.Y > 0.9)
		{
			return "Y正方向";
		}
		if (val.Y < -0.9)
		{
			return "Y负方向";
		}
		string text = ((val.X > 0.0) ? "东" : "西");
		string text2 = ((val.Y > 0.0) ? "北" : "南");
		return text + text2;
	}
}
