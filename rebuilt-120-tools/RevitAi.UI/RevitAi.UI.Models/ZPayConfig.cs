using System.ComponentModel;

namespace RevitAi.UI.Models;

public class ZPayConfig : INotifyPropertyChanged
{
	private string _merchantId = "2026020421162461";

	private string _merchantKey = "WKpdLqSA4y0j7QMGUSg395tMnHPgb4Lr";

	private string _apiGateway = "https://zpayz.cn/mapi.php";

	public string MerchantId
	{
		get
		{
			return _merchantId;
		}
		set
		{
			if (_merchantId != value)
			{
				_merchantId = value;
				OnPropertyChanged("MerchantId");
			}
		}
	}

	public string MerchantKey
	{
		get
		{
			return _merchantKey;
		}
		set
		{
			if (_merchantKey != value)
			{
				_merchantKey = value;
				OnPropertyChanged("MerchantKey");
			}
		}
	}

	public string ApiGateway
	{
		get
		{
			return _apiGateway;
		}
		set
		{
			if (_apiGateway != value)
			{
				_apiGateway = value;
				OnPropertyChanged("ApiGateway");
			}
		}
	}

	public string WechatChannelId { get; set; } = "12861";

	public string WechatMchId { get; set; } = "1106204604";

	public string AlipayChannelId { get; set; } = "12862";

	public string AlipayPartnerId { get; set; } = "2088202240984830";

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
