using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit.WallToRoad;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace RevitAi.UI.ViewModels;

public class WallToRoadViewModel : ObservableObject
{
	private readonly ILogger _logger;

	private readonly IRevitAdapter _revitAdapter;

	[ObservableProperty]
	private int selectedWallCount;

	[ObservableProperty]
	private double defaultRoadWidth = 9.0;

	[ObservableProperty]
	private double laneWidth = 4.25;

	[ObservableProperty]
	private double filletRadius = 10.0;

	[ObservableProperty]
	private double sidewalkWidth = 3.0;

	[ObservableProperty]
	private double curbWidth = 150.0;

	[ObservableProperty]
	private bool createSidewalks = true;

	[ObservableProperty]
	private bool createCurbs = true;

	[ObservableProperty]
	private bool createCenterMarkings = true;

	[ObservableProperty]
	private double centerMarkingWidth = 150.0;

	[ObservableProperty]
	private bool isCenterMarkingDoubleLine;

	[ObservableProperty]
	private bool createCrosswalks = true;

	[ObservableProperty]
	private double crosswalkLineWidth = 450.0;

	[ObservableProperty]
	private double crosswalkLineGap = 600.0;

	[ObservableProperty]
	private double crosswalkLength = 5.0;

	[ObservableProperty]
	private double crosswalkDistanceFromMarking = 1.0;

	[ObservableProperty]
	private bool createLaneMarkings = true;

	[ObservableProperty]
	private double laneMarkingWidth = 150.0;

	[ObservableProperty]
	private double laneMarkingSolidLength = 4.0;

	[ObservableProperty]
	private double laneMarkingGapLength = 6.0;

	[ObservableProperty]
	private bool createDirectionArrows = true;

	[ObservableProperty]
	private double arrowSpacing = 20.0;

	[ObservableProperty]
	private int arrowSizeIndex = 1;

	[ObservableProperty]
	private bool createSiteFloor = true;

	[ObservableProperty]
	private double siteFloorExtension = 20.0;

	[ObservableProperty]
	private bool deleteOriginalWalls;

	[ObservableProperty]
	private string statusMessage = "请选择墙，设置参数，然后点击创建道路";

	[ObservableProperty]
	private bool isProcessing;

	[ObservableProperty]
	private int progress;

	[ObservableProperty]
	private string progressText = "";

	private List<int> _selectedWallIds = new List<int>();

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? selectWallsCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? createRoadCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? closeCommand;

	public Action? CloseWindow { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int SelectedWallCount
	{
		get
		{
			return selectedWallCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(selectedWallCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedWallCount);
				selectedWallCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedWallCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double DefaultRoadWidth
	{
		get
		{
			return defaultRoadWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(defaultRoadWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DefaultRoadWidth);
				defaultRoadWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DefaultRoadWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double LaneWidth
	{
		get
		{
			return laneWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(laneWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LaneWidth);
				laneWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LaneWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double FilletRadius
	{
		get
		{
			return filletRadius;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(filletRadius, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.FilletRadius);
				filletRadius = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.FilletRadius);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double SidewalkWidth
	{
		get
		{
			return sidewalkWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(sidewalkWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SidewalkWidth);
				sidewalkWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SidewalkWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double CurbWidth
	{
		get
		{
			return curbWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(curbWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurbWidth);
				curbWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurbWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateSidewalks
	{
		get
		{
			return createSidewalks;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(createSidewalks, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateSidewalks);
				createSidewalks = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateSidewalks);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateCurbs
	{
		get
		{
			return createCurbs;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(createCurbs, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateCurbs);
				createCurbs = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateCurbs);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateCenterMarkings
	{
		get
		{
			return createCenterMarkings;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(createCenterMarkings, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateCenterMarkings);
				createCenterMarkings = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateCenterMarkings);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double CenterMarkingWidth
	{
		get
		{
			return centerMarkingWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(centerMarkingWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CenterMarkingWidth);
				centerMarkingWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CenterMarkingWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsCenterMarkingDoubleLine
	{
		get
		{
			return isCenterMarkingDoubleLine;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(isCenterMarkingDoubleLine, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsCenterMarkingDoubleLine);
				isCenterMarkingDoubleLine = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsCenterMarkingDoubleLine);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateCrosswalks
	{
		get
		{
			return createCrosswalks;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(createCrosswalks, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateCrosswalks);
				createCrosswalks = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateCrosswalks);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double CrosswalkLineWidth
	{
		get
		{
			return crosswalkLineWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(crosswalkLineWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CrosswalkLineWidth);
				crosswalkLineWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CrosswalkLineWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double CrosswalkLineGap
	{
		get
		{
			return crosswalkLineGap;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(crosswalkLineGap, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CrosswalkLineGap);
				crosswalkLineGap = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CrosswalkLineGap);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double CrosswalkLength
	{
		get
		{
			return crosswalkLength;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(crosswalkLength, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CrosswalkLength);
				crosswalkLength = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CrosswalkLength);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double CrosswalkDistanceFromMarking
	{
		get
		{
			return crosswalkDistanceFromMarking;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(crosswalkDistanceFromMarking, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CrosswalkDistanceFromMarking);
				crosswalkDistanceFromMarking = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CrosswalkDistanceFromMarking);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateLaneMarkings
	{
		get
		{
			return createLaneMarkings;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(createLaneMarkings, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateLaneMarkings);
				createLaneMarkings = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateLaneMarkings);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double LaneMarkingWidth
	{
		get
		{
			return laneMarkingWidth;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(laneMarkingWidth, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LaneMarkingWidth);
				laneMarkingWidth = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LaneMarkingWidth);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double LaneMarkingSolidLength
	{
		get
		{
			return laneMarkingSolidLength;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(laneMarkingSolidLength, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LaneMarkingSolidLength);
				laneMarkingSolidLength = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LaneMarkingSolidLength);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double LaneMarkingGapLength
	{
		get
		{
			return laneMarkingGapLength;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(laneMarkingGapLength, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LaneMarkingGapLength);
				laneMarkingGapLength = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LaneMarkingGapLength);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateDirectionArrows
	{
		get
		{
			return createDirectionArrows;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(createDirectionArrows, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateDirectionArrows);
				createDirectionArrows = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateDirectionArrows);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double ArrowSpacing
	{
		get
		{
			return arrowSpacing;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(arrowSpacing, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ArrowSpacing);
				arrowSpacing = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ArrowSpacing);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int ArrowSizeIndex
	{
		get
		{
			return arrowSizeIndex;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(arrowSizeIndex, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ArrowSizeIndex);
				arrowSizeIndex = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ArrowSizeIndex);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool CreateSiteFloor
	{
		get
		{
			return createSiteFloor;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(createSiteFloor, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreateSiteFloor);
				createSiteFloor = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreateSiteFloor);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public double SiteFloorExtension
	{
		get
		{
			return siteFloorExtension;
		}
		set
		{
			if (!EqualityComparer<double>.Default.Equals(siteFloorExtension, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SiteFloorExtension);
				siteFloorExtension = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SiteFloorExtension);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool DeleteOriginalWalls
	{
		get
		{
			return deleteOriginalWalls;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(deleteOriginalWalls, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DeleteOriginalWalls);
				deleteOriginalWalls = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DeleteOriginalWalls);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string StatusMessage
	{
		get
		{
			return statusMessage;
		}
		[MemberNotNull("statusMessage")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(statusMessage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.StatusMessage);
				statusMessage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.StatusMessage);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsProcessing
	{
		get
		{
			return isProcessing;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(isProcessing, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsProcessing);
				isProcessing = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsProcessing);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int Progress
	{
		get
		{
			return progress;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(progress, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Progress);
				progress = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Progress);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string ProgressText
	{
		get
		{
			return progressText;
		}
		[MemberNotNull("progressText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(progressText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ProgressText);
				progressText = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ProgressText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand SelectWallsCommand => selectWallsCommand ?? (selectWallsCommand = new AsyncRelayCommand(SelectWalls));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand CreateRoadCommand => createRoadCommand ?? (createRoadCommand = new AsyncRelayCommand(CreateRoad));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(Close));

	public WallToRoadViewModel()
	{
		IServiceProvider services = UIBootstrapper.Services;
		_logger = services.GetRequiredService<ILogger>();
		_revitAdapter = services.GetRequiredService<IRevitAdapter>();
	}

	[RelayCommand]
	private async Task SelectWalls()
	{
		try
		{
			IsProcessing = true;
			StatusMessage = "请在 Revit 中选择墙（可框选或单选）...";
			PropertyInfo property = _revitAdapter.GetType().GetProperty("SelectionService");
			if (property == null)
			{
				StatusMessage = "无法获取选择服务";
				return;
			}
			object value = property.GetValue(_revitAdapter);
			if (value == null)
			{
				StatusMessage = "选择服务为空";
				return;
			}
			MethodInfo method = value.GetType().GetMethod("SelectWallsAsync");
			if (method == null)
			{
				StatusMessage = "无法触发墙选择";
				return;
			}
			if (!(method.Invoke(value, new object[1] { "选择墙" }) is Task task))
			{
				StatusMessage = "选择墙方法返回 null";
				return;
			}
			await task;
			PropertyInfo property2 = task.GetType().GetProperty("Result");
			if (!(property2 != null))
			{
				return;
			}
			if (property2.GetValue(task) is List<object> { Count: >0 } list)
			{
				_selectedWallIds.Clear();
				foreach (object item in list)
				{
					PropertyInfo property3 = item.GetType().GetProperty("Id");
					if (!(property3 != null))
					{
						continue;
					}
					object value2 = property3.GetValue(item);
					if (value2 == null)
					{
						continue;
					}
					PropertyInfo property4 = value2.GetType().GetProperty("IntegerValue");
					if (property4 != null)
					{
						object value3 = property4.GetValue(value2);
						if (value3 != null)
						{
							_selectedWallIds.Add((int)value3);
						}
						continue;
					}
					PropertyInfo property5 = value2.GetType().GetProperty("Value");
					if (property5 != null)
					{
						object value4 = property5.GetValue(value2);
						if (value4 != null)
						{
							_selectedWallIds.Add((int)(long)value4);
						}
					}
				}
				SelectedWallCount = _selectedWallIds.Count;
				StatusMessage = $"已选择 {SelectedWallCount} 道墙";
			}
			else
			{
				SelectedWallCount = 0;
				StatusMessage = "未选择墙，请重新选择";
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[WallToRoadViewModel] 选择墙失败", ex);
			StatusMessage = "选择墙失败: " + ex.Message;
		}
		finally
		{
			IsProcessing = false;
		}
	}

	[RelayCommand]
	private async Task CreateRoad()
	{
		_ = 1;
		try
		{
			if (_selectedWallIds.Count == 0)
			{
				StatusMessage = "请先选择墙";
				return;
			}
			IsProcessing = true;
			StatusMessage = "正在创建道路网络...";
			Progress = 0;
			ProgressText = "正在提取墙中心线...";
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			WallToRoadRequest wallToRoadRequest = new WallToRoadRequest();
			wallToRoadRequest.WallElementIds = _selectedWallIds;
			wallToRoadRequest.DefaultRoadWidthMeters = DefaultRoadWidth;
			wallToRoadRequest.LaneWidthMeters = LaneWidth;
			wallToRoadRequest.FilletRadiusMeters = FilletRadius;
			wallToRoadRequest.IntersectionRadiusMeters = FilletRadius;
			wallToRoadRequest.SidewalkWidthMeters = SidewalkWidth;
			wallToRoadRequest.CurbWidthMillimeters = CurbWidth;
			wallToRoadRequest.CreateSidewalks = CreateSidewalks;
			wallToRoadRequest.CreateCurbs = CreateCurbs;
			wallToRoadRequest.CreateCenterMarkings = CreateCenterMarkings;
			wallToRoadRequest.CenterMarkingWidthMillimeters = CenterMarkingWidth;
			wallToRoadRequest.IsCenterMarkingDoubleLine = IsCenterMarkingDoubleLine;
			wallToRoadRequest.CreateCrosswalks = CreateCrosswalks;
			wallToRoadRequest.CrosswalkLineWidthMillimeters = CrosswalkLineWidth;
			wallToRoadRequest.CrosswalkSpacingMillimeters = CrosswalkLineWidth + CrosswalkLineGap;
			wallToRoadRequest.CrosswalkLengthMeters = CrosswalkLength;
			wallToRoadRequest.CrosswalkDistanceFromMarkingMeters = CrosswalkDistanceFromMarking;
			wallToRoadRequest.CreateLaneMarkings = CreateLaneMarkings;
			wallToRoadRequest.LaneMarkingWidthMillimeters = LaneMarkingWidth;
			wallToRoadRequest.LaneMarkingSolidLengthMeters = LaneMarkingSolidLength;
			wallToRoadRequest.LaneMarkingGapLengthMeters = LaneMarkingGapLength;
			wallToRoadRequest.CreateDirectionArrows = CreateDirectionArrows;
			wallToRoadRequest.ArrowSpacingMeters = ArrowSpacing;
			WallToRoadRequest wallToRoadRequest2 = wallToRoadRequest;
			wallToRoadRequest2.ArrowSizeMeters = ArrowSizeIndex switch
			{
				0 => 3.0, 
				1 => 6.0, 
				2 => 9.0, 
				_ => 6.0, 
			};
			wallToRoadRequest.CreateSiteFloor = CreateSiteFloor;
			wallToRoadRequest.SiteFloorExtensionMeters = SiteFloorExtension;
			wallToRoadRequest.DeleteOriginalWalls = DeleteOriginalWalls;
			wallToRoadRequest.OnCompleted = delegate(Exception? exception, bool success)
			{
				tcs.TrySetResult(success);
			};
			WallToRoadRequest request = wallToRoadRequest;
			MethodInfo method = _revitAdapter.GetType().GetMethod("TriggerWallToRoadOperation");
			if (method == null)
			{
				StatusMessage = "无法触发创建道路操作";
				IsProcessing = false;
				return;
			}
			method.Invoke(_revitAdapter, new object[1] { request });
			for (int i = 0; i < 100; i += 10)
			{
				Progress = i;
				ProgressText = $"正在处理... {i}%";
				await Task.Delay(100);
			}
			bool num = await tcs.Task;
			Progress = 100;
			ProgressText = "完成！";
			if (num)
			{
				int count = request.CreatedFloorIds.Count;
				StatusMessage = $"✓ 成功创建 {count} 个道路楼板";
				_logger.Info($"[WallToRoadViewModel] 成功创建 {count} 个道路楼板");
			}
			else
			{
				StatusMessage = "✗ 创建道路失败";
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[WallToRoadViewModel] 创建道路失败", ex);
			StatusMessage = "✗ 创建道路失败: " + ex.Message;
			ProgressText = "处理失败";
		}
		finally
		{
			IsProcessing = false;
		}
	}

	[RelayCommand]
	private void Close()
	{
		CloseWindow?.Invoke();
	}
}
