using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Windows;

public partial class AboutWindow : Window, IComponentConnector
{
	public AboutWindow()
	{
		try
		{
			LoadMaterialDesignResources();
		}
		catch (Exception)
		{
		}
		try
		{
			InitializeComponent();
		}
		catch (Exception ex2)
		{
			MessageBox.Show("初始化窗口失败：" + ex2.Message + "\n\n请检查是否缺少必要的依赖程序集。", "初始化错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			throw;
		}
		base.Topmost = true;
		try
		{
			LoadAppIcon();
		}
		catch (Exception)
		{
		}
		base.Loaded += AboutWindow_Loaded;
	}

	private async void AboutWindow_Loaded(object sender, RoutedEventArgs e)
	{
		_ = 1;
		try
		{
			await Task.Delay(50);
			if (base.DataContext is AboutViewModel aboutViewModel)
			{
				await aboutViewModel.InitializeDataAsync();
			}
		}
		catch (Exception)
		{
		}
	}

	private void LoadAppIcon()
	{
		try
		{
			string location = Assembly.GetExecutingAssembly().Location;
			if (string.IsNullOrEmpty(location))
			{
				return;
			}
			string directoryName = Path.GetDirectoryName(location);
			if (string.IsNullOrEmpty(directoryName))
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			for (int i = 0; i <= 2; i++)
			{
				string text = Path.Combine(directoryInfo.FullName, "Resources", "Icons", "ASTools.png");
				if (File.Exists(text))
				{
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.UriSource = new Uri(text, UriKind.Absolute);
					bitmapImage.EndInit();
					if (FindName("AppIconImage") is Image image)
					{
						image.Source = bitmapImage;
					}
					break;
				}
				if (directoryInfo.Parent != null)
				{
					directoryInfo = directoryInfo.Parent;
					continue;
				}
				break;
			}
		}
		catch (Exception)
		{
		}
	}

	private void LoadMaterialDesignResources()
	{
		try
		{
			base.Resources = new ResourceDictionary();
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == "MaterialDesignThemes.Wpf");
			if (assembly == null)
			{
				try
				{
					assembly = Assembly.Load("MaterialDesignThemes.Wpf");
				}
				catch
				{
					return;
				}
			}
			if (assembly != null)
			{
				Uri baseUri = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/");
				string[] array = new string[13]
				{
					"MaterialDesignTheme.Defaults.xaml", "MaterialDesignTheme.Light.xaml", "MaterialDesignTheme.TextBox.xaml", "MaterialDesignTheme.Button.xaml", "MaterialDesignTheme.PasswordBox.xaml", "MaterialDesignTheme.ComboBox.xaml", "MaterialDesignTheme.ToggleButton.xaml", "MaterialDesignTheme.CheckBox.xaml", "MaterialDesignTheme.TabControl.xaml", "MaterialDesignTheme.ProgressBar.xaml",
					"MaterialDesignTheme.Card.xaml", "MaterialDesignTheme.Shadows.xaml", "MaterialDesignTheme.ListBox.xaml"
				};
				foreach (string relativeUri in array)
				{
					try
					{
						Uri source = new Uri(baseUri, relativeUri);
						ResourceDictionary item = new ResourceDictionary
						{
							Source = source
						};
						base.Resources.MergedDictionaries.Add(item);
					}
					catch (Exception)
					{
					}
				}
			}
			Assembly assembly2 = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.GetName().Name == "MaterialDesignColors");
			if (assembly2 == null)
			{
				try
				{
					assembly2 = Assembly.Load("MaterialDesignColors");
				}
				catch
				{
					return;
				}
			}
			if (assembly2 != null)
			{
				try
				{
					Uri source2 = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml");
					ResourceDictionary item2 = new ResourceDictionary
					{
						Source = source2
					};
					base.Resources.MergedDictionaries.Add(item2);
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public string? GetPassword()
	{
		if (FindName("PasswordBox") is PasswordBox passwordBox)
		{
			return passwordBox.Password;
		}
		return null;
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
	}
}
