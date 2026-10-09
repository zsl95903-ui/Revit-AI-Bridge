using System;
using System.Linq;
using RevitAi.Core.AI.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAi.UI.ViewModels;

public class CodeSnippetItemViewModel : ObservableObject
{
	private readonly CodeSnippet _snippet;

	public string Id => _snippet.Id;

	public string Name => _snippet.Name;

	public string NameEmoji
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_snippet.Name))
			{
				return string.Empty;
			}
			string text = _snippet.Name.TrimStart();
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
			if (string.IsNullOrWhiteSpace(_snippet.Name))
			{
				return _snippet.Name;
			}
			string text = _snippet.Name.TrimStart();
			if (text.Length == 0)
			{
				return _snippet.Name;
			}
			int num = ((!char.IsSurrogatePair(text, 0)) ? 1 : 2);
			if (text.Length > num)
			{
				return text.Substring(num).TrimStart();
			}
			return string.Empty;
		}
	}

	public string Description => _snippet.Description;

	public string Code => _snippet.Code;

	public string TagsDisplay => string.Join(" ", _snippet.Tags.Select((string t) => "#" + t));

	public DateTime CreatedAt => _snippet.CreatedAt;

	public DateTime LastUsed => _snippet.LastUsed;

	public int UseCount => _snippet.UseCount;

	public string CreatedAtDisplay => _snippet.CreatedAt.ToString("yyyy-MM-dd");

	public string UseCountDisplay => $"使用 {UseCount} 次";

	public CodeSnippetItemViewModel(CodeSnippet snippet)
	{
		_snippet = snippet;
	}
}
