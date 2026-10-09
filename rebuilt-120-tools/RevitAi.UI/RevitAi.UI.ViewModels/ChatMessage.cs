using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.AI;
using RevitAi.UI.Models;

namespace RevitAi.UI.ViewModels;

public sealed class ChatMessage : INotifyPropertyChanged
{
	private string _content = string.Empty;

	private MessageKind _kind = MessageKind.AIResponse;

	public string Content
	{
		get
		{
			return _content;
		}
		set
		{
			_content = value;
			OnPropertyChanged("Content");
		}
	}

	public MessageKind Kind
	{
		get
		{
			return _kind;
		}
		set
		{
			_kind = value;
			OnPropertyChanged("Kind");
		}
	}

	public bool IsUser
	{
		get
		{
			return Kind == MessageKind.User;
		}
		set
		{
			Kind = ((!value) ? MessageKind.AIResponse : MessageKind.User);
		}
	}

	public DateTime Timestamp { get; set; } = DateTime.Now;

	public bool IsTyping { get; set; }

	public ToolCallInfo? ToolCall { get; set; }

	public List<FileAttachment>? Attachments { get; set; }

	public string? SavableCode { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
