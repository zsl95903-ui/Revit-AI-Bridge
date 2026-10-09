using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using RevitAi.Abstractions.Logging;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace RevitAi.UI.ViewModels;

public abstract class BridgeComponentDefinitionViewModelBase : ObservableObject
{
	[ObservableProperty]
	private BridgeComponentStyleItem? _selectedStyle;

	[ObservableProperty]
	private string _newComponentName = string.Empty;

	[ObservableProperty]
	private bool _isLoading;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<BridgeComponentStyleItem?>? selectStyleCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? addComponentCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand<UserDefinedBridgeComponent?>? removeComponentCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? importCustomStyleCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? saveCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	public abstract string ComponentTypeName { get; }

	public ObservableCollection<BridgeComponentStyleItem> AvailableStyles { get; } = new ObservableCollection<BridgeComponentStyleItem>();

	public ObservableCollection<UserDefinedBridgeComponent> UserComponents { get; } = new ObservableCollection<UserDefinedBridgeComponent>();

	public Action? RequestClose { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public BridgeComponentStyleItem? SelectedStyle
	{
		get
		{
			return _selectedStyle;
		}
		set
		{
			if (!EqualityComparer<BridgeComponentStyleItem>.Default.Equals(_selectedStyle, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedStyle);
				_selectedStyle = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedStyle);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string NewComponentName
	{
		get
		{
			return _newComponentName;
		}
		[MemberNotNull("_newComponentName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_newComponentName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.NewComponentName);
				_newComponentName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.NewComponentName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLoading
	{
		get
		{
			return _isLoading;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoading, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsLoading);
				_isLoading = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsLoading);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<BridgeComponentStyleItem?> SelectStyleCommand => selectStyleCommand ?? (selectStyleCommand = new RelayCommand<BridgeComponentStyleItem>(SelectStyle));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand AddComponentCommand => addComponentCommand ?? (addComponentCommand = new RelayCommand(AddComponent));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<UserDefinedBridgeComponent?> RemoveComponentCommand => removeComponentCommand ?? (removeComponentCommand = new RelayCommand<UserDefinedBridgeComponent>(RemoveComponent));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ImportCustomStyleCommand => importCustomStyleCommand ?? (importCustomStyleCommand = new RelayCommand(ImportCustomStyle));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand SaveCommand => saveCommand ?? (saveCommand = new RelayCommand(Save));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	[RelayCommand]
	private void SelectStyle(BridgeComponentStyleItem? style)
	{
		if (style != null)
		{
			SelectedStyle = style;
			Logger.Debug("选中样式: " + style.Name);
		}
	}

	protected BridgeComponentDefinitionViewModelBase()
	{
		LoadBuiltInStyles();
		LoadFromMemory();
		if (AvailableStyles.Count > 0)
		{
			SelectedStyle = AvailableStyles[0];
		}
	}

	protected virtual void LoadBuiltInStyles()
	{
		AvailableStyles.Clear();
		for (int i = 1; i <= 6; i++)
		{
			AvailableStyles.Add(new BridgeComponentStyleItem
			{
				Id = $"{ComponentTypeName}_Style_{i}",
				Name = $"{ComponentTypeName}样式 {i}",
				ImagePath = $"/RevitAi.UI;component/Resources/BridgeComponents/{ComponentTypeName}/Style{i}.png",
				IsBuiltIn = true,
				Description = $"{ComponentTypeName}样式 {i}的说明"
			});
		}
	}

	protected virtual void LoadFromMemory()
	{
		try
		{
			List<UserDefinedBridgeComponent> components = BridgeComponentDefinitionStore.Instance.GetComponents(ComponentTypeName);
			UserComponents.Clear();
			foreach (UserDefinedBridgeComponent component in components)
			{
				if (component.SelectedStyle != null)
				{
					BridgeComponentStyleItem bridgeComponentStyleItem = AvailableStyles.FirstOrDefault((BridgeComponentStyleItem s) => s.Id == component.SelectedStyle.Id);
					if (bridgeComponentStyleItem != null)
					{
						UserDefinedBridgeComponent item = new UserDefinedBridgeComponent
						{
							Id = component.Id,
							Name = component.Name,
							SelectedStyle = bridgeComponentStyleItem,
							ComponentType = ComponentTypeName,
							CreatedAt = component.CreatedAt
						};
						UserComponents.Add(item);
					}
				}
			}
			Logger.Info($"[{ComponentTypeName}] 已从内存加载 {UserComponents.Count} 个用户定义的部件");
		}
		catch (Exception ex)
		{
			Logger.Error("[" + ComponentTypeName + "] 从内存加载部件失败", ex);
		}
	}

	[RelayCommand]
	private void AddComponent()
	{
		try
		{
			if (SelectedStyle == null)
			{
				MessageBox.Show("请先选择一个样式", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (string.IsNullOrWhiteSpace(NewComponentName))
			{
				MessageBox.Show("请输入部件名称", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (UserComponents.Any((UserDefinedBridgeComponent c) => c.Name == NewComponentName))
			{
				MessageBox.Show("该名称已存在，请使用其他名称", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			UserDefinedBridgeComponent userDefinedBridgeComponent = new UserDefinedBridgeComponent
			{
				Name = NewComponentName,
				SelectedStyle = SelectedStyle,
				ComponentType = ComponentTypeName
			};
			UserComponents.Add(userDefinedBridgeComponent);
			NewComponentName = string.Empty;
			Logger.Info("添加" + ComponentTypeName + ": " + userDefinedBridgeComponent.Name);
		}
		catch (Exception ex)
		{
			Logger.Error("添加" + ComponentTypeName + "失败", ex);
			MessageBox.Show("添加失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void RemoveComponent(UserDefinedBridgeComponent? component)
	{
		try
		{
			if (component != null && MessageBox.Show("确定要删除部件 [" + component.Name + "] 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				UserComponents.Remove(component);
				Logger.Info("删除" + ComponentTypeName + ": " + component.Name);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("删除" + ComponentTypeName + "失败", ex);
			MessageBox.Show("删除失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void ImportCustomStyle()
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "选择部件样式图片",
				Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp|所有文件|*.*",
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() == true)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
				BridgeComponentStyleItem bridgeComponentStyleItem = new BridgeComponentStyleItem
				{
					Id = Guid.NewGuid().ToString(),
					Name = fileNameWithoutExtension,
					ImagePath = openFileDialog.FileName,
					IsBuiltIn = false,
					Description = "自定义样式: " + fileNameWithoutExtension
				};
				AvailableStyles.Add(bridgeComponentStyleItem);
				SelectedStyle = bridgeComponentStyleItem;
				Logger.Info("导入自定义样式: " + fileNameWithoutExtension);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("导入自定义样式失败", ex);
			MessageBox.Show("导入失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Save()
	{
		try
		{
			BridgeComponentDefinitionStore.Instance.SetComponents(ComponentTypeName, UserComponents.ToList());
			Logger.Info($"[{ComponentTypeName}] 已保存到内存: {UserComponents.Count} 个部件");
			RequestClose?.Invoke();
		}
		catch (Exception ex)
		{
			Logger.Error("[" + ComponentTypeName + "] 保存失败", ex);
			MessageBox.Show("保存失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		try
		{
			RequestClose?.Invoke();
		}
		catch (Exception ex)
		{
			Logger.Error("取消操作失败: " + ComponentTypeName, ex);
		}
	}

	public ObservableCollection<UserDefinedBridgeComponent> GetUserComponents()
	{
		return UserComponents;
	}
}
