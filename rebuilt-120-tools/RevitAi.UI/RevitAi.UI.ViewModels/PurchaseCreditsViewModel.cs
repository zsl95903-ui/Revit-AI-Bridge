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
using Microsoft.Extensions.DependencyInjection;

namespace RevitAi.UI.ViewModels;

public class PurchaseCreditsViewModel : ObservableObject
{
	private readonly Window _window;

	private CancellationTokenSource? _pollingCts;

	[ObservableProperty]
	private bool _isLoading;

	[ObservableProperty]
	private string _loadingText;

	[ObservableProperty]
	private ObservableCollection<CreditPackageOption> _creditPackages = new ObservableCollection<CreditPackageOption>();

	[ObservableProperty]
	private decimal _currentBalance;

	private decimal _customAmount;

	[ObservableProperty]
	private decimal _totalPrice;

	[ObservableProperty]
	private DiscountInfo? _discountInfo;

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

	private decimal _priceAfterSystemDiscount;

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

	private static IServiceProvider Services => UIBootstrapper.Services;

	private IDialogService DialogService => Services.GetRequiredService<IDialogService>();

	private ISupabaseClient? SupabaseClient => Services.GetService<ISupabaseClient>();

	private IPaymentService? PaymentService => Services.GetService<IPaymentService>();

	private IPaymentCompletionService? PaymentCompletionService => Services.GetService<IPaymentCompletionService>();

	public decimal CustomAmount
	{
		get
		{
			return _customAmount;
		}
		set
		{
			if (SetProperty(ref _customAmount, value, "CustomAmount"))
			{
				CustomAmountChanged?.Invoke(this, EventArgs.Empty);
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
	public ObservableCollection<CreditPackageOption> CreditPackages
	{
		get
		{
			return _creditPackages;
		}
		[MemberNotNull("_creditPackages")]
		set
		{
			if (!EqualityComparer<ObservableCollection<CreditPackageOption>>.Default.Equals(_creditPackages, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CreditPackages);
				_creditPackages = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CreditPackages);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal CurrentBalance
	{
		get
		{
			return _currentBalance;
		}
		set
		{
			if (!EqualityComparer<decimal>.Default.Equals(_currentBalance, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.CurrentBalance);
				_currentBalance = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.CurrentBalance);
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
	public DiscountInfo? DiscountInfo
	{
		get
		{
			return _discountInfo;
		}
		set
		{
			if (!EqualityComparer<RevitAi.UI.Models.DiscountInfo>.Default.Equals(_discountInfo, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DiscountInfo);
				_discountInfo = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DiscountInfo);
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

	public event EventHandler? CustomAmountChanged;

	public PurchaseCreditsViewModel(IDialogService? dialogService, Window window, ISupabaseClient? supabaseClient = null)
	{
		_window = window;
		IAuthManager service = Services.GetService<IAuthManager>();
		if (service != null)
		{
			try
			{
				_currentBalance = service.GetCachedCreditsBalance();
				Logger.Info($"[PurchaseCreditsViewModel] 从缓存加载电量余额: {_currentBalance:F2}");
			}
			catch (Exception ex)
			{
				Logger.Warning("[PurchaseCreditsViewModel] 从缓存获取电量余额失败: " + ex.Message);
				_currentBalance = 0m;
			}
		}
		else
		{
			_currentBalance = 0m;
		}
		_isLoading = false;
		_totalPrice = 0m;
		_loadingText = "处理中...";
		_isPaymentView = false;
		_customAmount = 10m;
		_showDeveloperQrCode = false;
		_developerQrCodeUrl = "pack://application:,,,/RevitAi.UI;component/Resources/Icons/wechat.jpg";
		CreditPackages = new ObservableCollection<CreditPackageOption>
		{
			new CreditPackageOption
			{
				DisplayName = "10元 - 20电量",
				OriginalPrice = 10m,
				Credits = 20,
				IsCustom = false
			},
			new CreditPackageOption
			{
				DisplayName = "20元 - 40电量",
				OriginalPrice = 20m,
				Credits = 40,
				IsCustom = false
			},
			new CreditPackageOption
			{
				DisplayName = "50元 - 100电量",
				OriginalPrice = 50m,
				Credits = 100,
				IsCustom = false
			},
			new CreditPackageOption
			{
				DisplayName = "100元 - 200电量",
				OriginalPrice = 100m,
				Credits = 200,
				IsCustom = false
			},
			new CreditPackageOption
			{
				DisplayName = "200元 - 400电量",
				OriginalPrice = 200m,
				Credits = 400,
				IsCustom = false
			},
			new CreditPackageOption
			{
				DisplayName = "500元 - 1000电量",
				OriginalPrice = 500m,
				Credits = 1000,
				IsCustom = false
			},
			new CreditPackageOption
			{
				DisplayName = "1000元 - 2000电量",
				OriginalPrice = 1000m,
				Credits = 2000,
				IsCustom = false
			},
			new CreditPackageOption
			{
				DisplayName = "自定义金额",
				OriginalPrice = 0m,
				Credits = 0,
				IsCustom = true
			}
		};
		CreditPackageOption creditPackageOption = CreditPackages.FirstOrDefault();
		if (creditPackageOption != null)
		{
			creditPackageOption.IsSelected = true;
			UpdatePriceAndDiscount(creditPackageOption);
		}
		foreach (CreditPackageOption option in CreditPackages)
		{
			option.PropertyChanged += delegate(object? s, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "IsSelected")
				{
					if (option.IsSelected)
					{
						foreach (CreditPackageOption creditPackage in CreditPackages)
						{
							if (creditPackage != option)
							{
								creditPackage.IsSelected = false;
							}
						}
						UpdatePriceAndDiscount(option);
					}
				}
				else if (e.PropertyName == "CustomAmount" && option.IsSelected && option.IsCustom)
				{
					UpdatePriceAndDiscount(option);
				}
			};
		}
		CustomAmountChanged += delegate
		{
			CreditPackageOption creditPackageOption2 = CreditPackages.FirstOrDefault((CreditPackageOption o) => o.IsCustom && o.IsSelected);
			if (creditPackageOption2 != null)
			{
				UpdatePriceAndDiscount(creditPackageOption2);
			}
		};
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
			ISupabaseClient supabaseClient = SupabaseClient;
			if (supabaseClient == null)
			{
				PromotionCodeError = "服务不可用";
				return;
			}
			IAuthManager service = Services.GetService<IAuthManager>();
			Guid? userId = service?.CurrentUser?.UserId;
			string deviceId = null;
			Result<IDeviceInfo> result = ((service == null) ? null : (await service.EnsureDeviceRegisteredAsync()));
			Result<IDeviceInfo> result2 = result;
			if (result2 != null && result2.IsSuccess && result2.Value != null)
			{
				deviceId = result2.Value.DeviceId;
			}
			Result<PromotionCodeValidationResult> result3 = await supabaseClient.ValidatePromotionCodeAsync(PromotionCodeInput, _priceAfterSystemDiscount, "credits", userId, deviceId);
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
				Logger.Warning("[PurchaseCredits] 优惠码验证失败: " + PromotionCodeInput + ", 错误: " + text);
				return;
			}
			PromotionCodeValidationResult value = result3.Value;
			if (!value.IsValid)
			{
				PromotionCodeError = ((!string.IsNullOrEmpty(value.ErrorMessage)) ? value.ErrorMessage : "优惠码无效、已过期或不满足使用条件");
				Logger.Info("[PurchaseCredits] 优惠码验证未通过: " + PromotionCodeInput + ", 原因: " + value.ErrorMessage);
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
				OriginalPrice = _priceAfterSystemDiscount,
				DiscountedPrice = (value.DiscountedPrice ?? _priceAfterSystemDiscount),
				SavedAmount = value.SavedAmount.GetValueOrDefault()
			};
			TotalPrice = AppliedPromotion.DiscountedPrice;
			PromotionCodeInput = string.Empty;
			Logger.Info($"[PurchaseCredits] 应用优惠码成功: {AppliedPromotion.Code}, 节省: ¥{AppliedPromotion.SavedAmount:F2}");
		}
		catch (Exception ex)
		{
			Logger.Error("[PurchaseCredits] 应用优惠码异常", ex);
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
		TotalPrice = _priceAfterSystemDiscount;
		Logger.Info("[PurchaseCredits] 清除优惠码");
	}

	[RelayCommand]
	private async Task ConfirmPurchaseAsync()
	{
		CreditPackageOption creditPackageOption = CreditPackages.FirstOrDefault((CreditPackageOption o) => o.IsSelected);
		if (creditPackageOption == null)
		{
			await DialogService.ShowErrorAsync("请选择电量套餐");
		}
		else
		{
			await PurchaseAsync(creditPackageOption);
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

	private DiscountInfo CalculateDiscount(decimal originalPrice, int originalCredits)
	{
		decimal num;
		string discountPercent;
		string description;
		if (originalPrice < 50m)
		{
			num = 1.0m;
			discountPercent = "无折扣";
			description = "满50元开始享受折扣";
		}
		else if (originalPrice < 100m)
		{
			num = 0.95m;
			discountPercent = "95折";
			description = "节省5%";
		}
		else if (originalPrice < 200m)
		{
			num = 0.9m;
			discountPercent = "9折";
			description = "节省10%";
		}
		else if (originalPrice < 500m)
		{
			num = 0.85m;
			discountPercent = "85折";
			description = "节省15%";
		}
		else if (originalPrice < 1000m)
		{
			num = 0.8m;
			discountPercent = "8折";
			description = "节省20%";
		}
		else
		{
			num = 0.75m;
			discountPercent = "75折";
			description = "节省25%";
		}
		decimal num2 = Math.Round(originalPrice * num, 2);
		decimal savedAmount = originalPrice - num2;
		return new DiscountInfo
		{
			DiscountRate = num,
			DiscountPercent = discountPercent,
			Description = description,
			OriginalPrice = originalPrice,
			DiscountedPrice = num2,
			SavedAmount = savedAmount,
			OriginalCredits = originalCredits,
			ActualCredits = originalCredits,
			BonusCredits = 0
		};
	}

	private void UpdatePriceAndDiscount(CreditPackageOption option)
	{
		decimal num;
		int originalCredits;
		if (option.IsCustom)
		{
			num = CustomAmount;
			originalCredits = (option.Credits = (int)(num * 2m));
		}
		else
		{
			num = option.OriginalPrice;
			originalCredits = option.Credits;
		}
		DiscountInfo discountInfo = (DiscountInfo = CalculateDiscount(num, originalCredits));
		_priceAfterSystemDiscount = discountInfo.DiscountedPrice;
		if (AppliedPromotion != null)
		{
			AppliedPromotion = null;
			TotalPrice = _priceAfterSystemDiscount;
		}
		else
		{
			TotalPrice = _priceAfterSystemDiscount;
		}
	}

	private async Task PurchaseAsync(CreditPackageOption selectedOption)
	{
		IsLoading = true;
		LoadingText = "正在创建订单...";
		try
		{
			if (SupabaseClient == null)
			{
				await DialogService.ShowErrorAsync("Supabase客户端未配置，请联系管理员。");
				IsLoading = false;
				return;
			}
			DiscountInfo discountInfo = DiscountInfo;
			if (discountInfo == null)
			{
				await DialogService.ShowErrorAsync("折扣信息计算失败");
				IsLoading = false;
				return;
			}
			decimal totalPrice = TotalPrice;
			int creditsToAdd = discountInfo.ActualCredits;
			string orderId = $"AST-CRD-{creditsToAdd:F0}-{DateTime.Now:yyyyMMddHHmmssfff}";
			LoadingText = "正在调用支付API...";
			IPaymentService paymentService = PaymentService;
			if (paymentService == null)
			{
				await DialogService.ShowErrorAsync("支付服务未配置");
				IsLoading = false;
				return;
			}
			PaymentResult paymentResult = await paymentService.CreatePaymentAsync($"RevitAi电量充值({creditsToAdd}电量)", totalPrice, orderId);
			if (!paymentResult.IsSuccess)
			{
				await DialogService.ShowErrorAsync("创建支付订单失败：" + paymentResult.Error);
				IsLoading = false;
				return;
			}
			PaymentOrderId = paymentResult.OrderId ?? orderId;
			AlipayQrCodeUrl = paymentResult.AlipayQrCodeUrl;
			WechatQrCodeUrl = paymentResult.WechatQrCodeUrl;
			IsLoading = false;
			IsPaymentView = true;
			Task.Run(() => PollPaymentStatusAsync(orderId, creditsToAdd));
		}
		catch (Exception ex)
		{
			Logger.Debug("[PurchaseCredits] 异常: " + ex.Message);
			Logger.Debug("[PurchaseCredits] 堆栈: " + ex.StackTrace);
			await DialogService.ShowErrorAsync("购买失败：" + ex.Message);
			IsLoading = false;
		}
	}

	private async Task PollPaymentStatusAsync(string orderId, int creditsToAdd)
	{
		_pollingCts = new CancellationTokenSource();
		try
		{
			IPaymentService paymentService = PaymentService;
			if (paymentService == null)
			{
				await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
				{
					await DialogService.ShowWarningAsync("支付服务不可用");
				});
				return;
			}
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
				PaymentStatusResult statusResult = await paymentService.QueryPaymentStatusAsync(orderId);
				if (!statusResult.IsSuccess)
				{
					await Task.Delay(3000, _pollingCts.Token);
					continue;
				}
				if (statusResult.Status == "success")
				{
					await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
					{
						await HandlePaymentSuccessAsync(creditsToAdd, statusResult);
					});
					return;
				}
				await Task.Delay(3000, _pollingCts.Token);
			}
			await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
			{
				await DialogService.ShowWarningAsync("支付检测超时。如果您已完成支付，请联系客服手动处理。");
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
				await DialogService.ShowErrorAsync("支付状态检测异常：" + ex3.Message);
			});
		}
		finally
		{
			_pollingCts?.Dispose();
			_pollingCts = null;
		}
	}

	private async Task HandlePaymentSuccessAsync(int creditsAdded, PaymentStatusResult paymentResult)
	{
		IsLoading = true;
		LoadingText = "正在充值电量...";
		try
		{
			DiscountInfo discountInfo = DiscountInfo;
			if (discountInfo == null)
			{
				await DialogService.ShowErrorAsync("折扣信息丢失");
				return;
			}
			if (PaymentCompletionService == null)
			{
				await DialogService.ShowErrorAsync("支付完成服务未配置，请联系管理员。");
				return;
			}
			string text = paymentResult.PaymentMethod ?? "alipay";
			string transactionId = paymentResult.TransactionId;
			Logger.Debug("[PurchaseCredits] 支付渠道: " + text + ", 交易ID: " + transactionId);
			Result result = await PaymentCompletionService.ProcessCreditRechargeAsync(PaymentOrderId, TotalPrice, creditsAdded, text, transactionId, discountInfo.OriginalPrice, discountInfo.OriginalCredits, discountInfo.DiscountRate);
			if (!result.IsSuccess)
			{
				await DialogService.ShowInfoAsync("支付成功！但充值失败：" + result.Error + "\n\n请联系客服，订单号：" + PaymentOrderId);
				return;
			}
			if (AppliedPromotion != null)
			{
				try
				{
					ISupabaseClient supabaseClient = SupabaseClient;
					if (supabaseClient != null)
					{
						IAuthManager service = Services.GetService<IAuthManager>();
						Guid? userId = service?.CurrentUser?.UserId;
						string deviceId = null;
						Result<IDeviceInfo> result2 = ((service == null) ? null : (await service.EnsureDeviceRegisteredAsync()));
						Result<IDeviceInfo> result3 = result2;
						if (result3 != null && result3.IsSuccess && result3.Value != null)
						{
							deviceId = result3.Value.DeviceId;
						}
						await supabaseClient.ActivatePromotionCodeAsync(AppliedPromotion.Code, PaymentOrderId, userId, deviceId);
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("[PurchaseCredits] 激活优惠码失败: " + ex.Message);
				}
			}
			await Task.Delay(500);
			IAuthManager authMgr = Services.GetService<IAuthManager>();
			if (authMgr != null)
			{
				Logger.Debug("[PurchaseCredits] 刷新授权状态...");
				try
				{
					await authMgr.RefreshAuthorizationDataAsync();
					Logger.Debug("[PurchaseCredits] 授权状态刷新完成");
					decimal cachedCreditsBalance = authMgr.GetCachedCreditsBalance();
					CurrentBalance = cachedCreditsBalance;
					await DialogService.ShowInfoAsync($"支付成功！已充值{creditsAdded}电量\n当前余额：{CurrentBalance:F1}电量");
				}
				catch (Exception ex2)
				{
					Logger.Warning("[PurchaseCredits] 刷新授权状态失败: " + ex2.Message);
				}
			}
			_window.DialogResult = true;
			_window.Close();
		}
		catch (Exception ex3)
		{
			await DialogService.ShowErrorAsync("支付成功，但处理异常：" + ex3.Message + "\n\n请联系客服，订单号：" + PaymentOrderId);
		}
		finally
		{
			IsLoading = false;
		}
	}
}
