using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Core.Services;
using RevitAi.UI.Services;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace RevitAi.UI.Views.Windows;

public partial class MapSelectionWindow : Window, IComponentConnector
{
	private bool _isClosed;

	private string? _tiandituToken;

	private string? _googleMapsApiKey;

	public BoundingBox? SelectedBoundingBox { get; private set; }

	public string SelectedMapSource { get; private set; } = "tianditu";

	public double? InitialLat { get; set; }

	public double? InitialLon { get; set; }

	public event EventHandler<MapSelectionCompletedEventArgs>? MapSelectionCompleted;

	public void ClearMapSelectionCompletedHandlers()
	{
		MapSelectionCompleted = null;
	}

	public MapSelectionWindow()
	{
		InitializeComponent();
		base.Closed += OnWindowClosed;
		InitializeAsync();
	}

	private void OnWindowClosed(object? sender, EventArgs e)
	{
		_isClosed = true;
		try
		{
			if (mapWebView?.CoreWebView2 != null)
			{
				mapWebView.WebMessageReceived -= MapWebView_WebMessageReceived;
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("[MapSelectionWindow] 移除事件处理器时异常: " + ex.Message);
		}
	}

	private async void InitializeAsync()
	{
		_ = 2;
		try
		{
			string systemProxyForWebView = GetSystemProxyForWebView2();
			string text = (string.IsNullOrEmpty(systemProxyForWebView) ? "no-proxy" : "with-proxy");
			string text2 = Path.Combine(Path.GetTempPath(), "RevitAi", "WebView2", text);
			try
			{
				if (Directory.Exists(text2))
				{
					Directory.Delete(text2, recursive: true);
				}
			}
			catch
			{
				text2 = Path.Combine(Path.GetTempPath(), "RevitAi", "WebView2", $"{text}_{DateTime.Now:yyyyMMdd_HHmmss}");
			}
			Directory.CreateDirectory(text2);
			CoreWebView2EnvironmentOptions coreWebView2EnvironmentOptions = new CoreWebView2EnvironmentOptions();
			if (!string.IsNullOrEmpty(systemProxyForWebView))
			{
				coreWebView2EnvironmentOptions.AdditionalBrowserArguments = systemProxyForWebView;
				Logger.Info("[MapSelectionWindow] 使用代理加载地图");
			}
			CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, text2, coreWebView2EnvironmentOptions);
			await mapWebView.EnsureCoreWebView2Async(environment);
			mapWebView.WebMessageReceived += MapWebView_WebMessageReceived;
			await LoadMapHtmlAsync();
		}
		catch (Exception ex)
		{
			Logger.Error("[MapSelectionWindow] WebView2 初始化失败: " + ex.Message);
			Logger.Error("[MapSelectionWindow] 堆栈跟踪: " + ex.StackTrace);
			MessageBox.Show("地图浏览器初始化失败:\n\n" + ex.Message + "\n\n请确保已安装 Microsoft Edge WebView2 运行时。", "初始化错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			btnConfirm.IsEnabled = false;
		}
	}

	private async Task LoadMapHtmlAsync()
	{
		_ = 1;
		try
		{
			string htmlContent = null;
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string path = ((directoryName != null) ? Path.Combine(directoryName, "Resources", "map.html") : string.Empty);
			if (File.Exists(path))
			{
				htmlContent = File.ReadAllText(path);
			}
			if (htmlContent == null)
			{
				string text = ((directoryName != null) ? Path.GetDirectoryName(directoryName) : null);
				string text2 = ((text != null) ? Path.Combine(text, "Resources", "map.html") : null);
				if (text2 != null && File.Exists(text2))
				{
					htmlContent = File.ReadAllText(text2);
				}
			}
			if (htmlContent == null)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RevitAi.UI.Resources.map.html");
				if (manifestResourceStream != null)
				{
					using StreamReader streamReader = new StreamReader(manifestResourceStream);
					htmlContent = streamReader.ReadToEnd();
				}
			}
			if (htmlContent == null)
			{
				throw new FileNotFoundException("找不到 map.html 文件");
			}
			_tiandituToken = await GetTiandituTokenAsync();
			htmlContent = htmlContent.Replace("YOUR_TIANDITU_TOKEN_HERE", _tiandituToken);
			_googleMapsApiKey = await GetGoogleMapsApiKeyAsync();
			if (InitialLat.HasValue && InitialLon.HasValue)
			{
				string text3 = $"\n                        <script>\n                            window.AS_TOOLS_INITIAL_POSITION = {{\n                                hasInitialPosition: true,\n                                lat: {InitialLat.Value.ToString(CultureInfo.InvariantCulture)},\n                                lon: {InitialLon.Value.ToString(CultureInfo.InvariantCulture)},\n                                zoom: 16\n                            }};\n                            console.log('检测到上次导入位置:', window.AS_TOOLS_INITIAL_POSITION);\n                        </script>\n                    ";
				htmlContent = htmlContent.Replace("</head>", text3 + "</head>");
				Logger.Info($"[MapSelectionWindow] 已注入上次导入位置: {InitialLat.Value}, {InitialLon.Value}");
			}
			else
			{
				string text4 = "\n                        <script>\n                            window.AS_TOOLS_INITIAL_POSITION = {\n                                hasInitialPosition: false,\n                                lat: 0,\n                                lon: 0,\n                                zoom: 13\n                            };\n                            console.log('无上次导入位置，将使用 IP 定位');\n                        </script>\n                    ";
				htmlContent = htmlContent.Replace("</head>", text4 + "</head>");
				Logger.Info("[MapSelectionWindow] 无上次导入位置，将使用 IP 定位");
			}
			mapWebView.CoreWebView2.NavigateToString(htmlContent);
		}
		catch (Exception ex)
		{
			Logger.Error("[MapSelectionWindow] 加载地图 HTML 失败: " + ex.Message);
			MessageBox.Show("加载地图失败: " + ex.Message, "加载错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private static async Task<string> GetTiandituTokenAsync()
	{
		try
		{
			IServiceProvider services = UIBootstrapper.Services;
			Type type = Type.GetType("RevitAi.Abstractions.Services.IApiKeyService, RevitAi.Abstractions");
			if (type != null)
			{
				object obj = typeof(IServiceProvider).GetMethod("GetService")?.Invoke(services, new object[1] { type });
				if (obj != null)
				{
					MethodInfo method = type.GetMethod("GetApiKeyAsync");
					if (method != null && method.Invoke(obj, new object[1] { "tianditu_token" }) is Task<string> task)
					{
						string text = await task;
						if (!string.IsNullOrEmpty(text) && text != "YOUR_TIANDITU_TOKEN_HERE")
						{
							return text;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[MapSelectionWindow] 获取 Tianditu Token 失败: " + ex.Message);
		}
		return "YOUR_TIANDITU_TOKEN_HERE";
	}

	private static async Task<string?> GetGoogleMapsApiKeyAsync()
	{
		try
		{
			IServiceProvider services = UIBootstrapper.Services;
			Type type = Type.GetType("RevitAi.Abstractions.Services.IApiKeyService, RevitAi.Abstractions");
			if (type != null)
			{
				object obj = typeof(IServiceProvider).GetMethod("GetService")?.Invoke(services, new object[1] { type });
				if (obj != null)
				{
					MethodInfo method = type.GetMethod("GetApiKeyAsync");
					if (method != null && method.Invoke(obj, new object[1] { "google_maps_api_key" }) is Task<string> task)
					{
						string text = await task;
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[MapSelectionWindow] 获取 Google Maps API Key 失败: " + ex.Message);
		}
		return null;
	}

	private static async Task<string?> GetAmapApiKeyAsync()
	{
		try
		{
			IServiceProvider services = UIBootstrapper.Services;
			Type type = Type.GetType("RevitAi.Abstractions.Services.IApiKeyService, RevitAi.Abstractions");
			if (type != null)
			{
				object obj = typeof(IServiceProvider).GetMethod("GetService")?.Invoke(services, new object[1] { type });
				if (obj != null)
				{
					MethodInfo method = type.GetMethod("GetAmapApiKeyAsync");
					if (method != null && method.Invoke(obj, new object[0]) is Task<string> task)
					{
						string text = await task;
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[MapSelectionWindow] 获取高德地图 API Key 失败: " + ex.Message);
		}
		return null;
	}

	private void MapWebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
	{
		if (_isClosed)
		{
			return;
		}
		try
		{
			string webMessageAsJson = e.WebMessageAsJson;
			dynamic val = JsonConvert.DeserializeObject(webMessageAsJson);
			if (val == null)
			{
				return;
			}
			switch ((string)(object)val.type)
			{
			case "mapSourceChanged":
				SelectedMapSource = val.source;
				break;
			case "coordinatesUpdated":
				SelectedMapSource = val.mapSource;
				SelectedBoundingBox = new BoundingBox
				{
					minLon = val.minLon,
					minLat = val.minLat,
					maxLon = val.maxLon,
					maxLat = val.maxLat
				};
				if (SelectedBoundingBox != null && SelectedBoundingBox.IsValid())
				{
					txtCoords.Text = SelectedBoundingBox.GetDisplayText();
					btnConfirm.IsEnabled = true;
				}
				else
				{
					Logger.Warning("[MapSelectionWindow] 收到无效的边界数据");
				}
				break;
			case "searchLocation":
			{
				string query = val.query;
				HandleSearchLocation(query);
				break;
			}
			default:
				SelectedBoundingBox = JsonConvert.DeserializeObject<BoundingBox>(webMessageAsJson);
				if (SelectedBoundingBox != null && SelectedBoundingBox.IsValid())
				{
					txtCoords.Text = SelectedBoundingBox.GetDisplayText();
					btnConfirm.IsEnabled = true;
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[MapSelectionWindow] 处理 Web 消息失败: " + ex.Message);
		}
	}

	private async void HandleSearchLocation(string query)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return;
			}
			Logger.Info("[MapSelectionWindow] C# 代理搜索位置: " + query);
			TiandituGeocodingService geocodingService = new TiandituGeocodingService();
			string token = _tiandituToken ?? "YOUR_TIANDITU_TOKEN_HERE";
			GeocodingResult geocodingResult = await Task.Run(() => geocodingService.Geocode(query, token));
			if (!_isClosed)
			{
				if (geocodingResult != null)
				{
					string value = geocodingResult.Latitude.ToString(CultureInfo.InvariantCulture);
					string value2 = geocodingResult.Longitude.ToString(CultureInfo.InvariantCulture);
					string value3 = geocodingResult.DisplayName?.Replace("'", "\\'") ?? query;
					mapWebView.CoreWebView2.ExecuteScriptAsync($"handleGeocodingResult(true, {value}, {value2}, '{value3}', '')");
					Logger.Info($"[MapSelectionWindow] 搜索成功: {geocodingResult.DisplayName} ({geocodingResult.Latitude}, {geocodingResult.Longitude})");
				}
				else
				{
					mapWebView.CoreWebView2.ExecuteScriptAsync("handleGeocodingResult(false, 0, 0, '', '未找到位置: " + query.Replace("'", "\\'") + "')");
					Logger.Warning("[MapSelectionWindow] 搜索失败: 未找到位置 " + query);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[MapSelectionWindow] 搜索异常: " + ex.Message);
			if (!_isClosed)
			{
				mapWebView.CoreWebView2.ExecuteScriptAsync("handleGeocodingResult(false, 0, 0, '', '" + ex.Message.Replace("'", "\\'") + "')");
			}
		}
	}

	private void BtnConfirm_Click(object sender, RoutedEventArgs e)
	{
		if (SelectedBoundingBox == null)
		{
			MessageBox.Show("请先在地图上拖动矩形选择一个区域！\n\n使用右上角的工具可以绘制新的矩形。", "未选择范围", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		if (!SelectedBoundingBox.IsValid())
		{
			MessageBox.Show("选择的范围无效！", "无效范围", MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		if (SelectedMapSource == "tianditu")
		{
			(int xCount, int yCount) tileCount = TileCalculator.GetTileCount(SelectedBoundingBox.minLon, SelectedBoundingBox.minLat, SelectedBoundingBox.maxLon, SelectedBoundingBox.maxLat, 18);
			int item = tileCount.xCount;
			int item2 = tileCount.yCount;
			int num = item * item2;
			if (num > 100 && MessageBox.Show($"当前选择的区域需要下载 {num} 个瓦片（{item} x {item2}）\n\n瓦片数量较多，下载可能需要较长时间。\n\n是否继续导入？", "天地图 - 瓦片数量确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
			{
				return;
			}
		}
		btnConfirm.IsEnabled = false;
		btnConfirm.Content = "正在导入卫星图，请稍候...";
		BoundingBox selectedBoundingBox = SelectedBoundingBox;
		string selectedMapSource = SelectedMapSource;
		MapSelectionCompleted?.Invoke(this, new MapSelectionCompletedEventArgs
		{
			BoundingBox = selectedBoundingBox,
			MapSource = selectedMapSource,
			TiandituToken = _tiandituToken,
			GoogleMapsApiKey = _googleMapsApiKey
		});
	}

	private void BtnCancel_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private static string? GetSystemProxyForWebView2()
	{
		try
		{
			Uri uri = new Uri("https://www.google.com");
			Uri proxy = WebRequest.GetSystemWebProxy().GetProxy(uri);
			if (proxy != null && !string.IsNullOrEmpty(proxy.Host) && proxy != uri)
			{
				return $"--proxy-server={proxy.Scheme}://{proxy.Host}:{proxy.Port}";
			}
			IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
			if (defaultWebProxy != null)
			{
				proxy = defaultWebProxy.GetProxy(uri);
				if (proxy != null && !string.IsNullOrEmpty(proxy.Host) && proxy != uri)
				{
					return $"--proxy-server={proxy.Scheme}://{proxy.Host}:{proxy.Port}";
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("[MapSelectionWindow] 检测系统代理失败: " + ex.Message);
		}
		Logger.Info("[MapSelectionWindow] 未检测到系统代理，WebView2 使用直连");
		return null;
	}
}
