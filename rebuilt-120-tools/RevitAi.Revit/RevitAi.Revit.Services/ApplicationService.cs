using System;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

public sealed class ApplicationService : IApplicationService
{
	private readonly UIApplication _uiApplication;

	public ApplicationService(UIApplication uiApplication)
	{
		_uiApplication = uiApplication ?? throw new ArgumentNullException("uiApplication");
	}

	public (byte red, byte green, byte blue) GetBackgroundColor()
	{
		try
		{
			Color backgroundColor = _uiApplication.Application.BackgroundColor;
			return (red: backgroundColor.Red, green: backgroundColor.Green, blue: backgroundColor.Blue);
		}
		catch (Exception ex)
		{
			Logger.Error("[ApplicationService] 获取背景色失败", ex);
			return (red: 0, green: 0, blue: 0);
		}
	}

	public void SetBackgroundColor(byte red, byte green, byte blue)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		try
		{
			Color backgroundColor = new Color(red, green, blue);
			_uiApplication.Application.BackgroundColor = backgroundColor;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[ApplicationService] 设置背景色为 RGB: (");
			defaultInterpolatedStringHandler.AppendFormatted(red);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(green);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(blue);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Error("[ApplicationService] 设置背景色失败", ex);
			throw;
		}
	}

	public (byte red, byte green, byte blue) ToggleBackgroundColor()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		try
		{
			Color backgroundColor = _uiApplication.Application.BackgroundColor;
			if (backgroundColor.Red == 0 && backgroundColor.Green == 0 && backgroundColor.Blue == 0)
			{
				Color backgroundColor2 = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
				_uiApplication.Application.BackgroundColor = backgroundColor2;
				Logger.Info("[ApplicationService] 切换背景色为白色");
				return (red: byte.MaxValue, green: byte.MaxValue, blue: byte.MaxValue);
			}
			Color backgroundColor3 = new Color((byte)0, (byte)0, (byte)0);
			_uiApplication.Application.BackgroundColor = backgroundColor3;
			Logger.Info("[ApplicationService] 切换背景色为黑色");
			return (red: 0, green: 0, blue: 0);
		}
		catch (Exception ex)
		{
			Logger.Error("[ApplicationService] 切换背景色失败", ex);
			throw;
		}
	}
}
