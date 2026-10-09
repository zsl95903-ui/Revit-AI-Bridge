using System.Windows;
using System.Windows.Controls;
using RevitAi.UI.ViewModels;

namespace RevitAi.UI.Views.Controls;

public class MessageTemplateSelector : DataTemplateSelector
{
	public DataTemplate? UserTemplate { get; set; }

	public DataTemplate? AITemplate { get; set; }

	public DataTemplate? AITypingTemplate { get; set; }

	public DataTemplate? AIThinkingTemplate { get; set; }

	public DataTemplate? AIToolCallTemplate { get; set; }

	public DataTemplate? AIToolResultTemplate { get; set; }

	public DataTemplate? AIToolErrorTemplate { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is ChatMessage chatMessage)
		{
			if (chatMessage.IsTyping)
			{
				return AITypingTemplate;
			}
			return chatMessage.Kind switch
			{
				MessageKind.User => UserTemplate, 
				MessageKind.AIThinking => AIThinkingTemplate, 
				MessageKind.AIToolCall => AIToolCallTemplate, 
				MessageKind.AIToolResult => AIToolResultTemplate, 
				MessageKind.AIToolError => AIToolErrorTemplate, 
				_ => AITemplate, 
			};
		}
		return base.SelectTemplate(item, container);
	}
}
