using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace RevitAi.UI.Views.Controls;

public class MarkdownTextBlock : FlowDocumentScrollViewer
{
	private readonly MarkdownPipeline _pipeline;

	public static readonly DependencyProperty MarkdownProperty;

	public string Markdown
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(MarkdownProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MarkdownProperty, (object)value);
		}
	}

	public MarkdownTextBlock()
	{
		_pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
		base.Document = new FlowDocument
		{
			PagePadding = new Thickness(0.0),
			TextAlignment = TextAlignment.Left,
			FontSize = 14.0,
			FontFamily = new FontFamily("Microsoft YaHei UI, Microsoft YaHei, SimHei, 宋体"),
			IsHyphenationEnabled = false
		};
		base.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
		base.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
		base.IsToolBarVisible = false;
		base.BorderThickness = new Thickness(0.0);
		base.Background = Brushes.Transparent;
		base.FontSize = 14.0;
		base.FontFamily = new FontFamily("Microsoft YaHei UI, Microsoft YaHei, SimHei, 宋体");
		base.Focusable = true;
		base.IsHitTestVisible = true;
		base.PreviewMouseWheel += OnPreviewMouseWheel;
	}

	private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer scrollViewer = FindParentScrollViewer((DependencyObject?)(object)this);
		if (scrollViewer != null && scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
		{
			MouseWheelEventArgs e2 = new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, e.Delta)
			{
				RoutedEvent = UIElement.MouseWheelEvent,
				Source = sender
			};
			scrollViewer.RaiseEvent(e2);
			e.Handled = true;
		}
	}

	private static ScrollViewer? FindParentScrollViewer(DependencyObject? obj)
	{
		if (obj == null)
		{
			return null;
		}
		DependencyObject parent = VisualTreeHelper.GetParent(obj);
		if (parent == null)
		{
			return null;
		}
		if (parent is ScrollViewer result)
		{
			return result;
		}
		return FindParentScrollViewer(parent);
	}

	private static void OnMarkdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is MarkdownTextBlock markdownTextBlock && ((DependencyPropertyChangedEventArgs)e).NewValue is string markdown)
		{
			markdownTextBlock.UpdateDocument(markdown);
		}
	}

	private void UpdateDocument(string markdown)
	{
		if (string.IsNullOrEmpty(markdown))
		{
			base.Document.Blocks.Clear();
			return;
		}
		try
		{
			MarkdownDocument markdownDocument = Markdig.Markdown.Parse(markdown, _pipeline);
			base.Document.Blocks.Clear();
			ConvertMarkdownToFlowDocument(markdownDocument);
		}
		catch
		{
			base.Document.Blocks.Clear();
			Paragraph item = new Paragraph(new Run(markdown))
			{
				Margin = new Thickness(0.0)
			};
			base.Document.Blocks.Add(item);
		}
	}

	private void ConvertMarkdownToFlowDocument(MarkdownDocument markdownDocument)
	{
		foreach (Markdig.Syntax.Block item in markdownDocument)
		{
			if (!(item is HeadingBlock headingBlock))
			{
				if (!(item is ParagraphBlock paragraphBlock))
				{
					if (!(item is ListBlock listBlock))
					{
						if (!(item is CodeBlock codeBlock))
						{
							if (!(item is QuoteBlock quoteBlock))
							{
								if (!(item is ThematicBreakBlock))
								{
									if (item is Markdig.Extensions.Tables.Table tableBlock)
									{
										ConvertTableBlock(tableBlock);
									}
								}
								else
								{
									AddHorizontalRule();
								}
							}
							else
							{
								ConvertQuoteBlock(quoteBlock);
							}
						}
						else
						{
							ConvertCodeBlock(codeBlock);
						}
					}
					else
					{
						ConvertListBlock(listBlock);
					}
				}
				else
				{
					ConvertParagraphBlock(paragraphBlock);
				}
			}
			else
			{
				ConvertHeadingBlock(headingBlock);
			}
		}
	}

	private void ConvertHeadingBlock(HeadingBlock headingBlock)
	{
		Paragraph paragraph = new Paragraph();
		paragraph.Margin = new Thickness(0.0);
		double num = (paragraph.FontSize = headingBlock.Level switch
		{
			1 => 18.0, 
			2 => 16.0, 
			3 => 15.0, 
			4 => 14.0, 
			5 => 13.0, 
			6 => 12.0, 
			_ => 13.0, 
		});
		paragraph.FontWeight = ((headingBlock.Level == 1) ? FontWeights.Bold : FontWeights.SemiBold);
		paragraph.Margin = new Thickness(0.0, num * 0.5, 0.0, num * 0.3);
		AddInlines(paragraph.Inlines, headingBlock.Inline);
		base.Document.Blocks.Add(paragraph);
	}

	private void ConvertParagraphBlock(ParagraphBlock paragraphBlock)
	{
		Paragraph paragraph = new Paragraph
		{
			Margin = new Thickness(0.0, 6.0, 0.0, 6.0)
		};
		AddInlines(paragraph.Inlines, paragraphBlock.Inline?.FirstChild);
		base.Document.Blocks.Add(paragraph);
	}

	private void ConvertListBlock(ListBlock listBlock)
	{
		List list = new List();
		list.Margin = new Thickness(20.0, 4.0, 0.0, 4.0);
		TextMarkerStyle markerStyle = ((!listBlock.IsOrdered) ? TextMarkerStyle.Disc : TextMarkerStyle.Decimal);
		list.MarkerStyle = markerStyle;
		foreach (ListItemBlock item in listBlock.OfType<ListItemBlock>())
		{
			ListItem listItem = new ListItem();
			Paragraph paragraph = new Paragraph
			{
				Margin = new Thickness(0.0)
			};
			foreach (Markdig.Syntax.Block item2 in item)
			{
				if (item2 is ParagraphBlock paragraphBlock)
				{
					AddInlines(paragraph.Inlines, paragraphBlock.Inline?.FirstChild);
				}
			}
			listItem.Blocks.Add(paragraph);
			list.ListItems.Add(listItem);
		}
		base.Document.Blocks.Add(list);
	}

	private void ConvertCodeBlock(CodeBlock codeBlock)
	{
		List<string> list = new List<string>();
		foreach (object line in codeBlock.Lines)
		{
			if (line != null)
			{
				list.Add(line.ToString() ?? string.Empty);
			}
		}
		Paragraph item = new Paragraph(new Run(string.Join("\n", list).Trim()))
		{
			Margin = new Thickness(8.0),
			Padding = new Thickness(8.0),
			Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
			FontFamily = new FontFamily("Consolas, Courier New, monospace"),
			FontSize = 13.0
		};
		base.Document.Blocks.Add(item);
	}

	private void ConvertQuoteBlock(QuoteBlock quoteBlock)
	{
		foreach (Markdig.Syntax.Block item in quoteBlock)
		{
			if (item is ParagraphBlock paragraphBlock)
			{
				Paragraph paragraph = new Paragraph
				{
					Margin = new Thickness(20.0, 6.0, 0.0, 6.0),
					Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
					FontStyle = FontStyles.Italic
				};
				AddInlines(paragraph.Inlines, paragraphBlock.Inline?.FirstChild);
				base.Document.Blocks.Add(paragraph);
			}
		}
	}

	private void AddHorizontalRule()
	{
		Paragraph item = new Paragraph(new Run())
		{
			BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
			BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0),
			Margin = new Thickness(0.0, 10.0, 0.0, 10.0)
		};
		base.Document.Blocks.Add(item);
	}

	private void AddInlines(InlineCollection inlines, Markdig.Syntax.Inlines.Inline? inline)
	{
		for (Markdig.Syntax.Inlines.Inline inline2 = inline; inline2 != null; inline2 = inline2.NextSibling)
		{
			if (!(inline2 is LiteralInline literalInline))
			{
				if (!(inline2 is CodeInline codeInline))
				{
					if (!(inline2 is EmphasisInline emphasisInline))
					{
						if (!(inline2 is LineBreakInline lineBreakInline))
						{
							if (!(inline2 is LinkInline linkInline))
							{
								if (inline2 is ContainerInline containerInline)
								{
									AddInlines(inlines, containerInline.FirstChild);
								}
								else if (inline2 is ContainerInline containerInline2)
								{
									AddInlines(inlines, containerInline2.FirstChild);
								}
							}
							else
							{
								Run run = new Run(string.Join("", linkInline.SelectMany((Markdig.Syntax.Inlines.Inline i) => ExtractTextRecursive(i))));
								if (!string.IsNullOrEmpty(linkInline.Url) && linkInline.Url != "http://" && linkInline.Url != "https://")
								{
									try
									{
										Hyperlink item = new Hyperlink(run)
										{
											NavigateUri = new Uri(linkInline.Url),
											Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 212))
										};
										inlines.Add(item);
									}
									catch
									{
										inlines.Add(run);
									}
								}
								else
								{
									inlines.Add(run);
								}
							}
						}
						else
						{
							inlines.Add(new LineBreak());
							if (lineBreakInline.IsHard)
							{
								inlines.Add(new LineBreak());
							}
						}
					}
					else
					{
						Span span = new Span();
						if (emphasisInline.DelimiterChar == '*' || emphasisInline.DelimiterChar == '_')
						{
							if (emphasisInline.DelimiterCount == 2)
							{
								span.FontWeight = FontWeights.Bold;
							}
							else
							{
								span.FontStyle = FontStyles.Italic;
							}
						}
						else if (emphasisInline.DelimiterChar == '~')
						{
							span.TextDecorations = TextDecorations.Strikethrough;
						}
						AddInlines(span.Inlines, emphasisInline.FirstChild);
						inlines.Add(span);
					}
				}
				else
				{
					Run item2 = new Run(codeInline.Content.ToString())
					{
						Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
						FontFamily = new FontFamily("Consolas, Courier New, monospace")
					};
					inlines.Add(item2);
				}
			}
			else
			{
				inlines.Add(new Run(literalInline.Content.ToString()));
			}
		}
	}

	private static string ExtractText(Markdig.Syntax.Inlines.Inline inline)
	{
		if (inline is LiteralInline literalInline)
		{
			return literalInline.Content.ToString();
		}
		if (inline is ContainerInline source)
		{
			return string.Join("", source.SelectMany((Markdig.Syntax.Inlines.Inline c) => ExtractTextRecursive(c)));
		}
		return string.Empty;
	}

	private static IEnumerable<string> ExtractTextRecursive(Markdig.Syntax.Inlines.Inline inline)
	{
		if (inline is LiteralInline literalInline)
		{
			yield return literalInline.Content.ToString();
		}
		else
		{
			if (!(inline is ContainerInline containerInline))
			{
				yield break;
			}
			foreach (Markdig.Syntax.Inlines.Inline item in containerInline)
			{
				foreach (string item2 in ExtractTextRecursive(item))
				{
					yield return item2;
				}
			}
		}
	}

	private void ConvertTableBlock(Markdig.Extensions.Tables.Table tableBlock)
	{
		System.Windows.Documents.Table table = new System.Windows.Documents.Table
		{
			Margin = new Thickness(0.0, 10.0, 0.0, 10.0),
			BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
			BorderThickness = new Thickness(1.0),
			CellSpacing = 0.0
		};
		int count = tableBlock.ColumnDefinitions.Count;
		for (int i = 0; i < count; i++)
		{
			TableColumn tableColumn = new TableColumn();
			tableColumn.Width = GridLength.Auto;
			table.Columns.Add(tableColumn);
		}
		TableRowGroup tableRowGroup = new TableRowGroup();
		table.RowGroups.Add(tableRowGroup);
		bool flag = true;
		foreach (Markdig.Syntax.Block item2 in tableBlock)
		{
			if (!(item2 is Markdig.Extensions.Tables.TableRow tableRow))
			{
				continue;
			}
			System.Windows.Documents.TableRow tableRow2 = new System.Windows.Documents.TableRow();
			if (flag)
			{
				tableRow2.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
				tableRow2.FontWeight = FontWeights.Bold;
				flag = false;
			}
			foreach (Markdig.Syntax.Block item3 in tableRow)
			{
				if (!(item3 is Markdig.Extensions.Tables.TableCell tableCell))
				{
					continue;
				}
				Paragraph paragraph = new Paragraph
				{
					Margin = new Thickness(8.0, 6.0, 8.0, 6.0),
					Padding = new Thickness(0.0)
				};
				foreach (Markdig.Syntax.Block item4 in tableCell)
				{
					if (item4 is ParagraphBlock paragraphBlock)
					{
						AddInlines(paragraph.Inlines, paragraphBlock.Inline?.FirstChild);
					}
				}
				System.Windows.Documents.TableCell item = new System.Windows.Documents.TableCell(paragraph)
				{
					BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
					BorderThickness = new Thickness(0.0, 0.0, 1.0, 1.0)
				};
				tableRow2.Cells.Add(item);
			}
			tableRowGroup.Rows.Add(tableRow2);
		}
		base.Document.Blocks.Add(table);
	}

	static MarkdownTextBlock()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		MarkdownProperty = DependencyProperty.Register("Markdown", typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnMarkdownChanged)));
	}
}
