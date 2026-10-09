using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevitAi.Abstractions.Authentication;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.Authentication;
using RevitAi.Core.Authentication.Models;
using RevitAi.UI.Models;
using RevitAi.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;

namespace RevitAi.UI.ViewModels;

public class PurchaseLicenseViewModel : ObservableObject
{
	private readonly IDialogService _dialogService;

	private readonly Window _window;

	private readonly IPaymentService? _paymentService;

	private readonly IPaymentCompletionService? _paymentCompletionService;

	private CancellationTokenSource? _pollingCts;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private string _loadingText;

	[ObservableProperty]
	private ObservableCollection<LicenseDurationOption> _licenseDurations = new ObservableCollection<LicenseDurationOption>();

	[ObservableProperty]
	private int _currentTokenBalance;

	[ObservableProperty]
	private int _selectedTokenPackage;

	[ObservableProperty]
	private decimal _totalPrice;

	[ObservableProperty]
	private string _paymentOrderId = string.Empty;

	[ObservableProperty]
	private string? _alipayQrCodeUrl;

	[ObservableProperty]
	private string? _wechatQrCodeUrl;

	[ObservableProperty]
	private string? _developerQrCodeUrl;

	[ObservableProperty]
	private bool _showDeveloperQrCode;

	[ObservableProperty]
	private bool _isPaymentView;

	[ObservableProperty]
	private string _promotionCodeInput = string.Empty;

	[ObservableProperty]
	private AppliedPromotionCode? _appliedPromotion;

	[ObservableProperty]
	private string? _promotionCodeError;

	[ObservableProperty]
	private bool _isValidatingPromoCode;

	private decimal _originalPrice;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? applyPromotionCodeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? clearPromotionCodeCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private AsyncRelayCommand? confirmPurchaseCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? backToSelectionCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	private RelayCommand? cancelPaymentCommand;

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

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string LoadingText
	{
		get
		{
			return _loadingText;
		}
		[MemberNotNull("_loadingText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_loadingText, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LoadingText);
				_loadingText = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LoadingText);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public ObservableCollection<LicenseDurationOption> LicenseDurations
	{
		get
		{
			return _licenseDurations;
		}
		[MemberNotNull("_licenseDurations")]
		set
		{
			if (!EqualityComparer<ObservableCollection<LicenseDurationOption>>.Default.Equals(_licenseDurations, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.LicenseDurations);
				_licenseDurations = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.LicenseDurations);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int CurrentTokenBalance
	{
		get
		{
			return _currentTokenBalance;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_currentTokenBalance, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentTokenBalance);
				_currentTokenBalance = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentTokenBalance);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int SelectedTokenPackage
	{
		get
		{
			return _selectedTokenPackage;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_selectedTokenPackage, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.SelectedTokenPackage);
				_selectedTokenPackage = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.SelectedTokenPackage);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal TotalPrice
	{
		get
		{
			return _totalPrice;
		}
		set
		{
			if (!EqualityComparer<decimal>.Default.Equals(_totalPrice, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.TotalPrice);
				_totalPrice = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.TotalPrice);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string PaymentOrderId
	{
		get
		{
			return _paymentOrderId;
		}
		[MemberNotNull("_paymentOrderId")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_paymentOrderId, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PaymentOrderId);
				_paymentOrderId = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PaymentOrderId);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? AlipayQrCodeUrl
	{
		get
		{
			return _alipayQrCodeUrl;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_alipayQrCodeUrl, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AlipayQrCodeUrl);
				_alipayQrCodeUrl = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AlipayQrCodeUrl);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? WechatQrCodeUrl
	{
		get
		{
			return _wechatQrCodeUrl;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_wechatQrCodeUrl, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.WechatQrCodeUrl);
				_wechatQrCodeUrl = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.WechatQrCodeUrl);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? DeveloperQrCodeUrl
	{
		get
		{
			return _developerQrCodeUrl;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_developerQrCodeUrl, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DeveloperQrCodeUrl);
				_developerQrCodeUrl = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DeveloperQrCodeUrl);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool ShowDeveloperQrCode
	{
		get
		{
			return _showDeveloperQrCode;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_showDeveloperQrCode, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ShowDeveloperQrCode);
				_showDeveloperQrCode = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ShowDeveloperQrCode);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsPaymentView
	{
		get
		{
			return _isPaymentView;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isPaymentView, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsPaymentView);
				_isPaymentView = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsPaymentView);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string PromotionCodeInput
	{
		get
		{
			return _promotionCodeInput;
		}
		[MemberNotNull("_promotionCodeInput")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_promotionCodeInput, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PromotionCodeInput);
				_promotionCodeInput = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PromotionCodeInput);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public AppliedPromotionCode? AppliedPromotion
	{
		get
		{
			return _appliedPromotion;
		}
		set
		{
			if (!EqualityComparer<AppliedPromotionCode>.Default.Equals(_appliedPromotion, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AppliedPromotion);
				_appliedPromotion = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AppliedPromotion);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string? PromotionCodeError
	{
		get
		{
			return _promotionCodeError;
		}
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_promotionCodeError, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.PromotionCodeError);
				_promotionCodeError = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.PromotionCodeError);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsValidatingPromoCode
	{
		get
		{
			return _isValidatingPromoCode;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isValidatingPromoCode, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsValidatingPromoCode);
				_isValidatingPromoCode = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsValidatingPromoCode);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(Cancel));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ApplyPromotionCodeCommand => applyPromotionCodeCommand ?? (applyPromotionCodeCommand = new AsyncRelayCommand(ApplyPromotionCodeAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClearPromotionCodeCommand => clearPromotionCodeCommand ?? (clearPromotionCodeCommand = new RelayCommand(ClearPromotionCode));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IAsyncRelayCommand ConfirmPurchaseCommand => confirmPurchaseCommand ?? (confirmPurchaseCommand = new AsyncRelayCommand(ConfirmPurchaseAsync));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand BackToSelectionCommand => backToSelectionCommand ?? (backToSelectionCommand = new RelayCommand(BackToSelection));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand CancelPaymentCommand => cancelPaymentCommand ?? (cancelPaymentCommand = new RelayCommand(CancelPayment));

	public PurchaseLicenseViewModel(IDialogService dialogService, Window window, IPaymentService? paymentService = null, IPaymentCompletionService? paymentCompletionService = null)
	{
		_dialogService = dialogService;
		_window = window;
		_paymentService = paymentService;
		_paymentCompletionService = paymentCompletionService;
		_isLoading = false;
		_currentTokenBalance = 0;
		_totalPrice = 0m;
		_loadingText = "处理中...";
		_isPaymentView = false;
		_showDeveloperQrCode = false;
		_developerQrCodeUrl = "pack://application:,,,/RevitAi.UI;component/Resources/Icons/wechat.jpg";
		LicenseDurations = new ObservableCollection<LicenseDurationOption>();
		Task.Run(() => LoadLicensePackagesAsync());
		LoadDefaultPackages();
		SetupOptionChangeHandlers();
	}

	private void LoadDefaultPackages()
	{
		LicenseDurations.Clear();
		LicenseDurations.Add(new LicenseDurationOption
		{
			DisplayName = "1个月 - ¥39",
			Months = 1,
			Price = 39m
		});
		LicenseDurations.Add(new LicenseDurationOption
		{
			DisplayName = "1个季度 - ¥99",
			Months = 3,
			Price = 99m
		});
		LicenseDurations.Add(new LicenseDurationOption
		{
			DisplayName = "半年 - ¥189",
			Months = 6,
			Price = 189m
		});
		LicenseDurations.Add(new LicenseDurationOption
		{
			DisplayName = "1年 - ¥329",
			Months = 12,
			Price = 329m
		});
		LicenseDurations.Add(new LicenseDurationOption
		{
			DisplayName = "永久授权 - ¥1499",
			Months = 999,
			Price = 1499m
		});
		LicenseDurationOption licenseDurationOption = LicenseDurations.FirstOrDefault();
		if (licenseDurationOption != null)
		{
			licenseDurationOption.IsSelected = true;
			TotalPrice = licenseDurationOption.Price;
		}
	}

	private async Task LoadLicensePackagesAsync()
	{
		try
		{
			ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
			if (supabaseClient == null)
			{
				Logger.Warning("[PurchaseLicense] 无法获取 SupabaseClient，使用默认套餐");
				return;
			}
			Result<List<LicensePackage>> result = await supabaseClient.GetLicensePackagesAsync();
			if (result.IsSuccess && result.Value != null && result.Value.Count > 0)
			{
				await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
				{
					LicenseDurations.Clear();
					int? previouslySelectedMonths = LicenseDurations.FirstOrDefault((LicenseDurationOption o) => o.IsSelected)?.Months;
					foreach (LicensePackage item in result.Value)
					{
						LicenseDurations.Add(new LicenseDurationOption
						{
							DisplayName = item.DisplayName,
							Months = item.Months,
							Price = item.Price
						});
					}
					SetupOptionChangeHandlers();
					LicenseDurationOption licenseDurationOption = LicenseDurations.FirstOrDefault((LicenseDurationOption o) => o.Months == previouslySelectedMonths) ?? LicenseDurations.FirstOrDefault();
					if (licenseDurationOption != null)
					{
						licenseDurationOption.IsSelected = true;
						TotalPrice = licenseDurationOption.Price;
					}
					Logger.Info($"[PurchaseLicense] 成功从数据库加载 {result.Value.Count} 个套餐");
				});
			}
			else
			{
				Logger.Warning("[PurchaseLicense] 数据库查询套餐失败: " + (result.Error ?? "未知错误") + "，使用默认套餐");
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[PurchaseLicense] 从数据库加载套餐失败: " + ex.Message + "，使用默认套餐");
		}
	}

	private void SetupOptionChangeHandlers()
	{
		foreach (LicenseDurationOption licenseDuration in LicenseDurations)
		{
			licenseDuration.PropertyChanged -= OnOptionPropertyChanged;
		}
		foreach (LicenseDurationOption licenseDuration2 in LicenseDurations)
		{
			licenseDuration2.PropertyChanged += OnOptionPropertyChanged;
		}
	}

	private void OnOptionPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (!(sender is LicenseDurationOption licenseDurationOption) || !(e.PropertyName == "IsSelected") || !licenseDurationOption.IsSelected)
		{
			return;
		}
		foreach (LicenseDurationOption licenseDuration in LicenseDurations)
		{
			if (licenseDuration != licenseDurationOption)
			{
				licenseDuration.IsSelected = false;
			}
		}
		_originalPrice = licenseDurationOption.Price;
		if (AppliedPromotion != null)
		{
			TotalPrice = _originalPrice;
			AppliedPromotion = null;
		}
		else
		{
			TotalPrice = _originalPrice;
		}
	}

	[RelayCommand]
	private void Cancel()
	{
		_window.DialogResult = false;
		_window.Close();
	}

	[RelayCommand]
	private async Task ApplyPromotionCodeAsync()
	{
		if (string.IsNullOrWhiteSpace(PromotionCodeInput))
		{
			PromotionCodeError = "请输入优惠码";
			return;
		}
		IsValidatingPromoCode = true;
		PromotionCodeError = null;
		try
		{
			ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
			if (supabaseClient == null)
			{
				PromotionCodeError = "服务不可用";
				return;
			}
			IAuthManager authManager = UIBootstrapper.TryGetService<IAuthManager>();
			Guid? userId = authManager?.CurrentUser?.UserId;
			string deviceId = null;
			Result<IDeviceInfo> result = ((authManager == null) ? null : (await authManager.EnsureDeviceRegisteredAsync()));
			Result<IDeviceInfo> result2 = result;
			if (result2 != null && result2.IsSuccess && result2.Value != null)
			{
				deviceId = result2.Value.DeviceId;
			}
			Result<PromotionCodeValidationResult> result3 = await supabaseClient.ValidatePromotionCodeAsync(PromotionCodeInput, (_originalPrice > 0m) ? _originalPrice : TotalPrice, "license", userId, deviceId);
			if (!result3.IsSuccess || result3.Value == null)
			{
				string text = result3.Error ?? "验证失败";
				if (text.Contains("not found") || text.Contains("404") || text.Contains("不存在"))
				{
					PromotionCodeError = "优惠码功能暂未开通，请联系客服";
				}
				else if (text.Contains("timeout") || text.Contains("timeout"))
				{
					PromotionCodeError = "网络超时，请稍后重试";
				}
				else
				{
					PromotionCodeError = "优惠码验证服务暂时不可用，请稍后重试";
				}
				Logger.Warning("[PurchaseLicense] 优惠码验证失败: " + PromotionCodeInput + ", 错误: " + text);
				return;
			}
			PromotionCodeValidationResult value = result3.Value;
			if (!value.IsValid)
			{
				PromotionCodeError = ((!string.IsNullOrEmpty(value.ErrorMessage)) ? value.ErrorMessage : "优惠码无效、已过期或不满足使用条件");
				Logger.Info("[PurchaseLicense] 优惠码验证未通过: " + PromotionCodeInput + ", 原因: " + value.ErrorMessage);
				return;
			}
			string description;
			if (value.PromotionCode.DiscountType.Equals("fixed", StringComparison.OrdinalIgnoreCase))
			{
				description = $"减{value.PromotionCode.DiscountValue}元";
			}
			else
			{
				decimal value2 = value.PromotionCode.DiscountValue / 10.0m;
				description = $"{value2:F1}折优惠";
			}
			AppliedPromotion = new AppliedPromotionCode
			{
				Code = value.PromotionCode.Code,
				DiscountType = value.PromotionCode.DiscountType,
				DiscountValue = value.PromotionCode.DiscountValue,
				Description = description,
				OriginalPrice = ((_originalPrice > 0m) ? _originalPrice : TotalPrice),
				DiscountedPrice = (value.DiscountedPrice ?? TotalPrice),
				SavedAmount = value.SavedAmount.GetValueOrDefault()
			};
			TotalPrice = AppliedPromotion.DiscountedPrice;
			PromotionCodeInput = string.Empty;
			Logger.Info($"[PurchaseLicense] 应用优惠码成功: {AppliedPromotion.Code}, 节省: ¥{AppliedPromotion.SavedAmount:F2}");
		}
		catch (Exception ex)
		{
			Logger.Error("[PurchaseLicense] 应用优惠码异常", ex);
			PromotionCodeError = "验证异常，请稍后重试";
		}
		finally
		{
			IsValidatingPromoCode = false;
		}
	}

	[RelayCommand]
	private void ClearPromotionCode()
	{
		AppliedPromotion = null;
		PromotionCodeError = null;
		TotalPrice = ((_originalPrice > 0m) ? _originalPrice : TotalPrice);
		Logger.Info("[PurchaseLicense] 清除优惠码");
	}

	[RelayCommand]
	private async Task ConfirmPurchaseAsync()
	{
		LicenseDurationOption licenseDurationOption = LicenseDurations.FirstOrDefault((LicenseDurationOption o) => o.IsSelected);
		if (licenseDurationOption == null)
		{
			await _dialogService.ShowErrorAsync("请选择授权套餐");
		}
		else
		{
			await PurchaseAsync(licenseDurationOption);
		}
	}

	[RelayCommand]
	private void BackToSelection()
	{
		_pollingCts?.Cancel();
		AlipayQrCodeUrl = null;
		WechatQrCodeUrl = null;
		PaymentOrderId = string.Empty;
		IsPaymentView = false;
	}

	[RelayCommand]
	private void CancelPayment()
	{
		_pollingCts?.Cancel();
		IsPaymentView = false;
		IsLoading = false;
	}

	private int ConvertMonthsToHours(int months)
	{
		if (months >= 999)
		{
			return -1;
		}
		return months * 744;
	}

	private async Task PurchaseAsync(LicenseDurationOption selectedOption)
	{
		IsLoading = true;
		LoadingText = "正在创建订单...";
		try
		{
			if (_paymentService == null)
			{
				await _dialogService.ShowErrorAsync("支付服务未配置，请联系管理员。");
				IsLoading = false;
				return;
			}
			string orderId = $"AST-SUB-{selectedOption.Months}-{DateTime.Now:yyyyMMddHHmmssfff}";
			LoadingText = "正在调用支付API...";
			PaymentResult paymentResult = await _paymentService.CreatePaymentAsync("RevitAi授权(" + selectedOption.DisplayName + ")", TotalPrice, orderId);
			if (!paymentResult.IsSuccess)
			{
				await _dialogService.ShowErrorAsync("创建支付订单失败：" + (paymentResult.Error ?? "未知错误"));
				IsLoading = false;
				return;
			}
			PaymentOrderId = paymentResult.OrderId ?? orderId;
			AlipayQrCodeUrl = paymentResult.AlipayQrCodeUrl;
			WechatQrCodeUrl = paymentResult.WechatQrCodeUrl;
			Logger.Debug("[Purchase] 订单号: " + PaymentOrderId);
			Logger.Debug("[Purchase] 支付宝二维码URL: " + AlipayQrCodeUrl);
			Logger.Debug("[Purchase] 微信二维码URL: " + WechatQrCodeUrl);
			Logger.Debug($"[Purchase] 二维码是否为空: {string.IsNullOrEmpty(AlipayQrCodeUrl)}");
			IsLoading = false;
			IsPaymentView = true;
			Task.Run(() => PollPaymentStatusAsync(orderId, selectedOption));
		}
		catch (Exception ex)
		{
			Logger.Debug("[Purchase] 异常: " + ex.Message);
			Logger.Debug("[Purchase] 堆栈: " + ex.StackTrace);
			await _dialogService.ShowErrorAsync("购买失败：" + ex.Message);
			IsLoading = false;
		}
	}

	private async Task PollPaymentStatusAsync(string orderId, LicenseDurationOption selectedOption)
	{
		_pollingCts = new CancellationTokenSource();
		try
		{
			for (int i = 0; i < 60; i++)
			{
				if (_pollingCts.Token.IsCancellationRequested)
				{
					return;
				}
				await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync((Action)delegate
				{
					LoadingText = $"检测支付状态... ({i * 3}秒)";
				});
				if (_paymentService == null)
				{
					await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
					{
						await _dialogService.ShowWarningAsync("支付服务不可用");
					});
					return;
				}
				PaymentStatusResult statusResult = await _paymentService.QueryPaymentStatusAsync(orderId);
				if (!statusResult.IsSuccess)
				{
					await Task.Delay(3000, _pollingCts.Token);
					continue;
				}
				if (statusResult.Status == "success")
				{
					await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
					{
						await HandlePaymentSuccessAsync(orderId, selectedOption, statusResult);
					});
					return;
				}
				await Task.Delay(3000, _pollingCts.Token);
			}
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
			{
				await _dialogService.ShowWarningAsync("支付检测超时。如果您已完成支付，请联系客服手动处理。");
			});
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
			{
				await _dialogService.ShowErrorAsync("支付状态检测异常：" + ex3.Message);
			});
		}
		finally
		{
			_pollingCts?.Dispose();
			_pollingCts = null;
		}
	}

	private async Task HandlePaymentSuccessAsync(string orderId, LicenseDurationOption selectedOption, PaymentStatusResult paymentResult)
	{
		IsLoading = true;
		LoadingText = "正在激活授权...";
		try
		{
			if (_paymentCompletionService == null)
			{
				await _dialogService.ShowWarningAsync("支付成功！但授权激活服务不可用，请联系客服手动激活。");
				return;
			}
			Result result = await _paymentCompletionService.ProcessLicensePurchaseAsync(orderId, selectedOption.Months, paymentResult.PaymentMethod ?? "alipay", paymentResult.PaidAmount, paymentResult.TransactionId);
			if (!result.IsSuccess)
			{
				await _dialogService.ShowErrorAsync("支付成功，但激活失败：" + result.Error + "\n\n请联系客服，订单号：" + orderId);
				return;
			}
			if (AppliedPromotion != null)
			{
				try
				{
					ISupabaseClient supabaseClient = UIBootstrapper.TryGetService<ISupabaseClient>();
					if (supabaseClient != null)
					{
						IAuthManager authManager = UIBootstrapper.TryGetService<IAuthManager>();
						Guid? userId = authManager?.CurrentUser?.UserId;
						string deviceId = null;
						Result<IDeviceInfo> result2 = ((authManager == null) ? null : (await authManager.EnsureDeviceRegisteredAsync()));
						Result<IDeviceInfo> result3 = result2;
						if (result3 != null && result3.IsSuccess && result3.Value != null)
						{
							deviceId = result3.Value.DeviceId;
						}
						await supabaseClient.ActivatePromotionCodeAsync(AppliedPromotion.Code, orderId, userId, deviceId);
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[PurchaseLicense] 激活优惠码失败: " + ex.Message);
				}
			}
			string message = ((selectedOption.Months >= 999) ? "支付成功！永久授权已激活" : ("支付成功！" + selectedOption.DisplayName + "授权已激活"));
			await _dialogService.ShowInfoAsync(message);
			await Task.Delay(500);
			IAuthManager authManager2 = UIBootstrapper.TryGetService<IAuthManager>();
			if (authManager2 != null)
			{
				Logger.Debug("[Purchase] 刷新授权状态...");
				try
				{
					await authManager2.RefreshAuthorizationDataAsync();
					Logger.Debug("[Purchase] 授权状态刷新完成");
				}
				catch (Exception ex2)
				{
					Logger.Warning("[Purchase] 刷新授权状态失败: " + ex2.Message);
				}
			}
			_window.DialogResult = true;
			_window.Close();
		}
		catch (Exception ex3)
		{
			await _dialogService.ShowErrorAsync("支付成功，但处理异常：" + ex3.Message + "\n\n请联系客服，订单号：" + orderId);
		}
		finally
		{
			IsLoading = false;
		}
	}
}
