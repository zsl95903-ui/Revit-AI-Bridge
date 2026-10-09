using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RevitAi.Abstractions.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;

namespace RevitAi.UI.ViewModels;

public class CodeSnippetMarketItemViewModel : ObservableObject
{
	[ObservableProperty]
	private string _id = string.Empty;

	[ObservableProperty]
	private string _authorId = string.Empty;

	[ObservableProperty]
	private string _authorName = string.Empty;

	[ObservableProperty]
	private string _name = string.Empty;

	[ObservableProperty]
	private string _description = string.Empty;

	[ObservableProperty]
	private List<string> _tags = new List<string>();

	[ObservableProperty]
	private string _category = string.Empty;

	[ObservableProperty]
	private decimal _price;

	[ObservableProperty]
	private bool _isFree;

	[ObservableProperty]
	private int _downloadCount;

	[ObservableProperty]
	private int _viewCount;

	[ObservableProperty]
	private int _favoriteCount;

	[ObservableProperty]
	private decimal _averageRating;

	[ObservableProperty]
	private int _ratingCount;

	[ObservableProperty]
	private bool _isNew;

	[ObservableProperty]
	private bool _isFavorited;

	[ObservableProperty]
	private bool _isOwned;

	[ObservableProperty]
	private int? _earnings;

	[ObservableProperty]
	private string _status = "active";

	public string NameEmoji
	{
		get
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				return string.Empty;
			}
			string text = Name.TrimStart();
			if (text.Length == 0)
			{
				return string.Empty;
			}
			if (char.IsSurrogatePair(text, 0) && text.Length >= 2)
			{
				return text.Substring(0, 2);
			}
			return text.Substring(0, 1);
		}
	}

	public string NameText
	{
		get
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				return Name;
			}
			string text = Name.TrimStart();
			if (text.Length == 0)
			{
				return Name;
			}
			int num = ((!char.IsSurrogatePair(text, 0)) ? 1 : 2);
			if (text.Length > num)
			{
				return text.Substring(num).TrimStart();
			}
			return string.Empty;
		}
	}

	public string CategoryDisplayName => Category switch
	{
		"modeling" => "建模工具", 
		"annotation" => "标注工具", 
		"analysis" => "分析工具", 
		"view" => "视图工具", 
		"documentation" => "文档工具", 
		"utility" => "实用工具", 
		"other" => "其他", 
		_ => "未知", 
	};

	public string StatusDisplayName => Status switch
	{
		"active" => "已上架", 
		"suspended" => "已暂停", 
		"deleted" => "已下架", 
		_ => "未知", 
	};

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Id
	{
		get
		{
			return _id;
		}
		[MemberNotNull("_id")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_id, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Id);
				_id = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Id);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string AuthorId
	{
		get
		{
			return _authorId;
		}
		[MemberNotNull("_authorId")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_authorId, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AuthorId);
				_authorId = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AuthorId);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string AuthorName
	{
		get
		{
			return _authorName;
		}
		[MemberNotNull("_authorName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_authorName, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AuthorName);
				_authorName = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AuthorName);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Name
	{
		get
		{
			return _name;
		}
		[MemberNotNull("_name")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_name, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Name);
				_name = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Name);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Description
	{
		get
		{
			return _description;
		}
		[MemberNotNull("_description")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_description, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Description);
				_description = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Description);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public List<string> Tags
	{
		get
		{
			return _tags;
		}
		[MemberNotNull("_tags")]
		set
		{
			if (!EqualityComparer<List<string>>.Default.Equals(_tags, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Tags);
				_tags = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Tags);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Category
	{
		get
		{
			return _category;
		}
		[MemberNotNull("_category")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_category, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Category);
				_category = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Category);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal Price
	{
		get
		{
			return _price;
		}
		set
		{
			if (!EqualityComparer<decimal>.Default.Equals(_price, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Price);
				_price = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Price);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsFree
	{
		get
		{
			return _isFree;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isFree, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsFree);
				_isFree = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsFree);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int DownloadCount
	{
		get
		{
			return _downloadCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_downloadCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.DownloadCount);
				_downloadCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.DownloadCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int ViewCount
	{
		get
		{
			return _viewCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_viewCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.ViewCount);
				_viewCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.ViewCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int FavoriteCount
	{
		get
		{
			return _favoriteCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_favoriteCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.FavoriteCount);
				_favoriteCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.FavoriteCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public decimal AverageRating
	{
		get
		{
			return _averageRating;
		}
		set
		{
			if (!EqualityComparer<decimal>.Default.Equals(_averageRating, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.AverageRating);
				_averageRating = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.AverageRating);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int RatingCount
	{
		get
		{
			return _ratingCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_ratingCount, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.RatingCount);
				_ratingCount = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.RatingCount);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsNew
	{
		get
		{
			return _isNew;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isNew, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsNew);
				_isNew = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsNew);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsFavorited
	{
		get
		{
			return _isFavorited;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isFavorited, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsFavorited);
				_isFavorited = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsFavorited);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsOwned
	{
		get
		{
			return _isOwned;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isOwned, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.IsOwned);
				_isOwned = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.IsOwned);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public int? Earnings
	{
		get
		{
			return _earnings;
		}
		set
		{
			if (!EqualityComparer<int?>.Default.Equals(_earnings, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Earnings);
				_earnings = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Earnings);
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.3.0.0")]
	[ExcludeFromCodeCoverage]
	public string Status
	{
		get
		{
			return _status;
		}
		[MemberNotNull("_status")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_status, value))
			{
				OnPropertyChanging(__KnownINotifyPropertyChangingArgs.Status);
				_status = value;
				OnPropertyChanged(__KnownINotifyPropertyChangedArgs.Status);
			}
		}
	}

	public CodeSnippetMarketItemViewModel(CodeSnippetMarketItem item, bool isFavorited = false, bool isOwned = false)
	{
		Id = item.Id;
		AuthorId = item.AuthorId;
		AuthorName = item.AuthorName;
		Name = item.Name;
		Description = item.Description;
		Tags = item.Tags;
		Category = item.Category;
		Price = item.Price;
		IsFree = item.IsFree;
		DownloadCount = item.DownloadCount;
		ViewCount = item.ViewCount;
		FavoriteCount = item.FavoriteCount;
		AverageRating = item.AverageRating;
		RatingCount = item.RatingCount;
		IsNew = item.IsNew;
		Earnings = item.Earnings;
		Status = item.Status ?? "active";
		IsFavorited = isFavorited;
		IsOwned = isOwned;
	}
}
