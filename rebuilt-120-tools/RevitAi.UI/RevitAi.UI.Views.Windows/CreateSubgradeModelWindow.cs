using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class CreateSubgradeModelWindow : Window, IComponentConnector
{
	private bool _isDragging;

	private bool _isPlanDragging;

	private bool _isPlanRotating;

	private Point _lastMousePosition;

	private Point _lastPlanMousePosition;

	private Point _rotationStartMousePosition;

	private double _rotationStartAngle;

	private double _rotationStartTranslateX;

	private double _rotationStartTranslateY;

	public static readonly DependencyProperty InverseScaleProperty;

	public static readonly DependencyProperty InverseRotationAngleProperty;

	public double InverseScale
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(InverseScaleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InverseScaleProperty, (object)value);
		}
	}

	public double InverseRotationAngle
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(InverseRotationAngleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InverseRotationAngleProperty, (object)value);
		}
	}

	public CreateSubgradeModelWindow()
	{
		InitializeComponent();
		InverseScale = 1.0;
		InverseRotationAngle = 0.0;
	}

	private void PreviewCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!(base.DataContext is CreateSubgradeModelViewModel createSubgradeModelViewModel))
		{
			return;
		}
		Size newSize = e.NewSize;
		if (((Size)newSize).Width > 0.0)
		{
			newSize = e.NewSize;
			if (((Size)newSize).Height > 0.0)
			{
				newSize = e.NewSize;
				double width = ((Size)newSize).Width;
				newSize = e.NewSize;
				createSubgradeModelViewModel.UpdatePreviewSize(width, ((Size)newSize).Height);
			}
		}
	}

	private void PreviewCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (PreviewScale != null && PreviewTranslate != null && PreviewContent != null)
		{
			double num = ((e.Delta > 0) ? 1.15 : 0.8695652173913044);
			double num2 = PreviewScale.ScaleX * num;
			double num3 = PreviewScale.ScaleY * num;
			if (!(num2 < 0.1) && !(num2 > 20.0) && !(num3 < 0.1) && !(num3 > 20.0))
			{
				Point position = e.GetPosition(PreviewCanvas);
				double num4 = (((Point)position).X - PreviewTranslate.X) / (PreviewContent.Width * PreviewScale.ScaleX);
				double num5 = (((Point)position).Y - PreviewTranslate.Y) / (PreviewContent.Height * PreviewScale.ScaleY);
				PreviewScale.ScaleX = num2;
				PreviewScale.ScaleY = num3;
				PreviewTranslate.X = ((Point)position).X - num4 * PreviewContent.Width * num2;
				PreviewTranslate.Y = ((Point)position).Y - num5 * PreviewContent.Height * num3;
				UpdateStrokeThickness(num2);
				e.Handled = true;
			}
		}
	}

	private void PreviewCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ClickCount == 2)
		{
			ResetZoom();
			e.Handled = true;
		}
		else
		{
			StartDrag(e);
		}
	}

	private void PreviewCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Middle)
		{
			if (e.ClickCount == 2)
			{
				ResetZoom();
				e.Handled = true;
			}
			else if (e.ButtonState == MouseButtonState.Pressed)
			{
				StartDrag(e);
			}
		}
	}

	private void StartDrag(MouseButtonEventArgs e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (PreviewCanvas != null)
		{
			_isDragging = true;
			_lastMousePosition = e.GetPosition(PreviewCanvas);
			PreviewCanvas.CaptureMouse();
			e.Handled = true;
		}
	}

	private void PreviewCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		StopDrag();
	}

	private void PreviewCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Middle)
		{
			StopDrag();
		}
	}

	private void StopDrag()
	{
		if (_isDragging && PreviewCanvas != null)
		{
			_isDragging = false;
			PreviewCanvas.ReleaseMouseCapture();
		}
	}

	private void PreviewCanvas_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (_isDragging && PreviewCanvas != null && PreviewTranslate != null)
		{
			Point position = e.GetPosition(PreviewCanvas);
			Vector val = position - _lastMousePosition;
			PreviewTranslate.X += ((Vector)val).X;
			PreviewTranslate.Y += ((Vector)val).Y;
			_lastMousePosition = position;
			e.Handled = true;
		}
	}

	private void ResetZoom()
	{
		if (PreviewScale != null && PreviewTranslate != null && PreviewContent != null && PreviewCanvas != null)
		{
			double actualWidth = PreviewCanvas.ActualWidth;
			double actualHeight = PreviewCanvas.ActualHeight;
			double width = PreviewContent.Width;
			double height = PreviewContent.Height;
			if (!(actualWidth <= 0.0) && !(actualHeight <= 0.0) && !(width <= 0.0) && !(height <= 0.0))
			{
				int num = 20;
				double val = (actualWidth - (double)num) / width;
				double val2 = (actualHeight - (double)num) / height;
				double num2 = Math.Min(val, val2);
				PreviewScale.ScaleX = num2;
				PreviewScale.ScaleY = num2;
				PreviewTranslate.X = (actualWidth - width * num2) / 2.0;
				PreviewTranslate.Y = (actualHeight - height * num2) / 2.0;
				UpdateStrokeThickness(num2);
			}
		}
	}

	private void PlanPreviewCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!(base.DataContext is CreateSubgradeModelViewModel createSubgradeModelViewModel))
		{
			return;
		}
		Size newSize = e.NewSize;
		if (((Size)newSize).Width > 0.0)
		{
			newSize = e.NewSize;
			if (((Size)newSize).Height > 0.0)
			{
				newSize = e.NewSize;
				double width = ((Size)newSize).Width;
				newSize = e.NewSize;
				createSubgradeModelViewModel.UpdatePlanPreviewSize(width, ((Size)newSize).Height);
			}
		}
	}

	private void PlanPreviewCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (PlanPreviewScale != null && PlanPreviewTranslate != null && PlanPreviewRotate != null && PlanPreviewContent != null && base.DataContext is CreateSubgradeModelViewModel createSubgradeModelViewModel)
		{
			double num = ((e.Delta > 0) ? 1.15 : 0.8695652173913044);
			double scaleX = PlanPreviewScale.ScaleX;
			double scaleY = PlanPreviewScale.ScaleY;
			double num2 = scaleX * num;
			double num3 = scaleY * num;
			if (!(num2 < 0.1) && !(num2 > 20.0) && !(num3 < 0.1) && !(num3 > 20.0))
			{
				Point position = e.GetPosition(PlanPreviewCanvas);
				double num4 = ((Point)position).X - PlanPreviewTranslate.X;
				double num5 = ((Point)position).Y - PlanPreviewTranslate.Y;
				double num6 = (0.0 - PlanPreviewRotate.Angle) * Math.PI / 180.0;
				double num7 = Math.Cos(num6);
				double num8 = Math.Sin(num6);
				double num9 = num4 * num7 - num5 * num8;
				double num10 = num4 * num8 + num5 * num7;
				double num11 = num9 / scaleX;
				double num12 = num10 / scaleY;
				double num13 = num11 * num2;
				double num14 = num12 * num3;
				double num15 = PlanPreviewRotate.Angle * Math.PI / 180.0;
				double num16 = Math.Cos(num15);
				double num17 = Math.Sin(num15);
				double num18 = num13 * num16 - num14 * num17;
				double num19 = num13 * num17 + num14 * num16;
				double x = ((Point)position).X - num18;
				double y = ((Point)position).Y - num19;
				PlanPreviewScale.ScaleX = num2;
				PlanPreviewScale.ScaleY = num3;
				PlanPreviewTranslate.X = x;
				PlanPreviewTranslate.Y = y;
				InverseScale = ((num2 > 0.0) ? (1.0 / num2) : 1.0);
				InverseRotationAngle = 0.0 - PlanPreviewRotate.Angle;
				createSubgradeModelViewModel.PlanPreviewViewState.Scale = num2;
				UpdatePlanStrokeThickness(num2);
				e.Handled = true;
			}
		}
	}

	private void PlanPreviewCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ClickCount == 2)
		{
			ResetPlanZoom();
			e.Handled = true;
		}
		else
		{
			StartPlanDrag(e);
		}
	}

	private void PlanPreviewCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Middle)
		{
			if (e.ClickCount == 2)
			{
				ResetPlanZoom();
				e.Handled = true;
			}
			else if (e.ButtonState == MouseButtonState.Pressed)
			{
				StartPlanDrag(e);
			}
		}
	}

	private void StartPlanDrag(MouseButtonEventArgs e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (PlanPreviewCanvas != null)
		{
			_isPlanDragging = true;
			_lastPlanMousePosition = e.GetPosition(PlanPreviewCanvas);
			PlanPreviewCanvas.CaptureMouse();
			e.Handled = true;
		}
	}

	private void PlanPreviewCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		StopPlanDrag();
	}

	private void PlanPreviewCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (PlanPreviewCanvas != null && PlanPreviewRotate != null && PlanPreviewTranslate != null && PlanPreviewScale != null)
		{
			_isPlanRotating = true;
			_lastPlanMousePosition = e.GetPosition(PlanPreviewCanvas);
			_rotationStartMousePosition = _lastPlanMousePosition;
			_rotationStartAngle = PlanPreviewRotate.Angle;
			_rotationStartTranslateX = PlanPreviewTranslate.X;
			_rotationStartTranslateY = PlanPreviewTranslate.Y;
			PlanPreviewCanvas.CaptureMouse();
			e.Handled = true;
		}
	}

	private void PlanPreviewCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
	{
		StopPlanRotation();
	}

	private void PlanPreviewCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Middle)
		{
			StopPlanDrag();
		}
	}

	private void StopPlanRotation()
	{
		if (_isPlanRotating && PlanPreviewCanvas != null)
		{
			_isPlanRotating = false;
			PlanPreviewCanvas.ReleaseMouseCapture();
		}
	}

	private void StopPlanDrag()
	{
		if (_isPlanDragging && PlanPreviewCanvas != null)
		{
			_isPlanDragging = false;
			PlanPreviewCanvas.ReleaseMouseCapture();
		}
	}

	private void PlanPreviewCanvas_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (_isPlanDragging && PlanPreviewCanvas != null && PlanPreviewTranslate != null)
		{
			Point position = e.GetPosition(PlanPreviewCanvas);
			Vector val = position - _lastPlanMousePosition;
			PlanPreviewTranslate.X += ((Vector)val).X;
			PlanPreviewTranslate.Y += ((Vector)val).Y;
			_lastPlanMousePosition = position;
			e.Handled = true;
		}
		else if (_isPlanRotating && PlanPreviewCanvas != null && PlanPreviewRotate != null && PlanPreviewTranslate != null && PlanPreviewScale != null)
		{
			Point position2 = e.GetPosition(PlanPreviewCanvas);
			double num = ((Point)position2).X - ((Point)_rotationStartMousePosition).X;
			double num2 = ((Point)position2).Y - ((Point)_rotationStartMousePosition).Y;
			if (Math.Abs(num) < 1.0 && Math.Abs(num2) < 1.0)
			{
				_lastPlanMousePosition = position2;
				e.Handled = true;
				return;
			}
			double num3 = Math.Atan2(num2, num) * 180.0 / Math.PI;
			num3 *= 0.2;
			double num4 = _rotationStartAngle + num3;
			double num5 = ((Point)_rotationStartMousePosition).X - _rotationStartTranslateX;
			double num6 = ((Point)_rotationStartMousePosition).Y - _rotationStartTranslateY;
			double num7 = (0.0 - _rotationStartAngle) * Math.PI / 180.0;
			double num8 = Math.Cos(num7);
			double num9 = Math.Sin(num7);
			double num10 = num5 * num8 - num6 * num9;
			double num11 = num5 * num9 + num6 * num8;
			double num12 = num10 / PlanPreviewScale.ScaleX;
			double num13 = num11 / PlanPreviewScale.ScaleY;
			PlanPreviewRotate.Angle = num4;
			double num14 = num12 * PlanPreviewScale.ScaleX;
			double num15 = num13 * PlanPreviewScale.ScaleY;
			double num16 = num4 * Math.PI / 180.0;
			double num17 = Math.Cos(num16);
			double num18 = Math.Sin(num16);
			double num19 = num14 * num17 - num15 * num18;
			double num20 = num14 * num18 + num15 * num17;
			double x = ((Point)_rotationStartMousePosition).X - num19;
			double y = ((Point)_rotationStartMousePosition).Y - num20;
			PlanPreviewTranslate.X = x;
			PlanPreviewTranslate.Y = y;
			InverseRotationAngle = 0.0 - PlanPreviewRotate.Angle;
			_lastPlanMousePosition = position2;
			e.Handled = true;
		}
	}

	private void ResetPlanZoom()
	{
		if (PlanPreviewScale != null && PlanPreviewTranslate != null && PlanPreviewRotate != null && PlanPreviewContent != null && PlanPreviewCanvas != null && base.DataContext is CreateSubgradeModelViewModel createSubgradeModelViewModel)
		{
			double actualWidth = PlanPreviewCanvas.ActualWidth;
			double actualHeight = PlanPreviewCanvas.ActualHeight;
			double width = PlanPreviewContent.Width;
			double height = PlanPreviewContent.Height;
			if (!(actualWidth <= 0.0) && !(actualHeight <= 0.0) && !(width <= 0.0) && !(height <= 0.0))
			{
				int num = 20;
				double val = (actualWidth - (double)num) / width;
				double val2 = (actualHeight - (double)num) / height;
				double num2 = Math.Min(val, val2);
				PlanPreviewScale.ScaleX = num2;
				PlanPreviewScale.ScaleY = num2;
				PlanPreviewTranslate.X = (actualWidth - width * num2) / 2.0;
				PlanPreviewTranslate.Y = (actualHeight - height * num2) / 2.0;
				PlanPreviewRotate.Angle = 0.0;
				InverseScale = ((num2 > 0.0) ? (1.0 / num2) : 1.0);
				InverseRotationAngle = 0.0;
				createSubgradeModelViewModel.PlanPreviewViewState.Scale = num2;
				UpdatePlanStrokeThickness(num2);
			}
		}
	}

	private void UpdateStrokeThickness(double scale)
	{
		if (!(base.DataContext is CreateSubgradeModelViewModel createSubgradeModelViewModel))
		{
			return;
		}
		double num = 0.8;
		double val = ((scale > 0.0) ? (num / scale) : num);
		val = Math.Max(0.2, Math.Min(3.0, val));
		foreach (LayerPreviewData layerPreviewDatum in createSubgradeModelViewModel.LayerPreviewData)
		{
			layerPreviewDatum.StrokeThickness = val;
		}
	}

	private void UpdatePlanStrokeThickness(double scale)
	{
		if (base.DataContext is CreateSubgradeModelViewModel createSubgradeModelViewModel)
		{
			double num = 2.0;
			double val = ((scale > 0.0) ? (num / scale) : num);
			val = Math.Max(0.5, Math.Min(5.0, val));
			if (createSubgradeModelViewModel.PlanLayoutPreviewData != null)
			{
				createSubgradeModelViewModel.PlanLayoutPreviewData.StrokeThickness = val;
				double num2 = 8.0;
				double num3 = 9.0;
				double val2 = ((scale > 0.0) ? (num2 / scale) : num2);
				double val3 = ((scale > 0.0) ? (num3 / scale) : num3);
				val2 = Math.Max(3.0, Math.Min(20.0, val2));
				val3 = Math.Max(4.0, val3);
				createSubgradeModelViewModel.PlanLayoutPreviewData.MarkerSize = val2;
				createSubgradeModelViewModel.PlanLayoutPreviewData.FontSize = val3;
			}
		}
	}

	private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_ = base.DataContext is CreateSubgradeModelViewModel;
	}

	static CreateSubgradeModelWindow()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		InverseScaleProperty = DependencyProperty.Register("InverseScale", typeof(double), typeof(CreateSubgradeModelWindow), new PropertyMetadata((object)1.0));
		InverseRotationAngleProperty = DependencyProperty.Register("InverseRotationAngle", typeof(double), typeof(CreateSubgradeModelWindow), new PropertyMetadata((object)0.0));
	}
}
