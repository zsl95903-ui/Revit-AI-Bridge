using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Core.AI.Models;
using Newtonsoft.Json;
using ns7;

namespace RevitAi.Core.AI;

public sealed class CodeSnippetStorageService
{
	[CompilerGenerated]
	public sealed class Class116
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Id == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class117
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Name.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class118
	{
		public CodeSnippet codeSnippet_0;

		internal bool method_0(CodeSnippet codeSnippet_1)
		{
			return codeSnippet_1.Id == codeSnippet_0.Id;
		}
	}

	[CompilerGenerated]
	public sealed class Class119
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Id == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class120
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Id == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class121
	{
		public string string_0;

		public Func<string, bool> func_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			if (!codeSnippet_0.Name.ToLower().Contains(string_0) && !codeSnippet_0.Description.ToLower().Contains(string_0) && !codeSnippet_0.Tags.Any((string string_1) => string_1.ToLower().Contains(string_0)))
			{
				return codeSnippet_0.Code.ToLower().Contains(string_0);
			}
			return true;
		}

		internal bool method_1(string string_1)
		{
			return string_1.ToLower().Contains(string_0);
		}
	}

	[CompilerGenerated]
	public sealed class Class122
	{
		public string string_0;

		public Func<string, bool> func_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Tags.Any((string string_1) => string_1.Equals(string_0, StringComparison.OrdinalIgnoreCase));
		}

		internal bool method_1(string string_1)
		{
			return string_1.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class Class123
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Id == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class124
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Id == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class125
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Id == string_0;
		}
	}

	[CompilerGenerated]
	public sealed class Class126
	{
		public string string_0;

		internal bool method_0(CodeSnippet codeSnippet_0)
		{
			return codeSnippet_0.Id == string_0;
		}
	}

	private static readonly string string_0 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "AI");

	private static readonly string string_1 = Path.Combine(string_0, "SavedCodeSnippets.json");

	private readonly object object_0 = new object();

	private CodeSnippetLibrary codeSnippetLibrary_0 = new CodeSnippetLibrary();

	public CodeSnippetStorageService()
	{
		method_0();
	}

	private void method_0()
	{
		try
		{
			if (!File.Exists(string_1))
			{
				Logger.Info("[CodeSnippetStorage] 首次使用，创建新库: " + string_1);
				method_2();
				return;
			}
			string text = File.ReadAllText(string_1);
			codeSnippetLibrary_0 = JsonConvert.DeserializeObject<CodeSnippetLibrary>(text) ?? new CodeSnippetLibrary();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippetStorage] 加载成功，已保存 ");
			defaultInterpolatedStringHandler.AppendFormatted(codeSnippetLibrary_0.Snippets.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个代码片段");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetStorage] 加载失败: " + ex.Message, ex);
			codeSnippetLibrary_0 = new CodeSnippetLibrary();
		}
	}

	public void Reload()
	{
		lock (object_0)
		{
			method_0();
		}
	}

	private void method_1()
	{
		try
		{
			method_2();
			string contents = JsonConvert.SerializeObject((object)codeSnippetLibrary_0, (Formatting)1);
			File.WriteAllText(string_1, contents);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippetStorage] 保存成功，当前 ");
			defaultInterpolatedStringHandler.AppendFormatted(codeSnippetLibrary_0.Snippets.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个代码片段");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Error("[CodeSnippetStorage] 保存失败: " + ex.Message, ex);
		}
	}

	private void method_2()
	{
		if (!Directory.Exists(string_0))
		{
			Directory.CreateDirectory(string_0);
		}
	}

	public List<CodeSnippet> GetAllSnippets(bool sortByOrder = false)
	{
		lock (object_0)
		{
			if (sortByOrder)
			{
				return (from codeSnippet_0 in codeSnippetLibrary_0.Snippets
					orderby (codeSnippet_0.Order != int.MaxValue) ? codeSnippet_0.Order : int.MaxValue, codeSnippet_0.LastUsed descending
					select codeSnippet_0).ToList();
			}
			return codeSnippetLibrary_0.Snippets.OrderByDescending((CodeSnippet codeSnippet_0) => codeSnippet_0.LastUsed).ToList();
		}
	}

	public CodeSnippet? GetSnippet(string id)
	{
		lock (object_0)
		{
			return codeSnippetLibrary_0.Snippets.FirstOrDefault((CodeSnippet codeSnippet_0) => codeSnippet_0.Id == id);
		}
	}

	public CodeSnippet? GetSnippetByName(string name)
	{
		lock (object_0)
		{
			return codeSnippetLibrary_0.Snippets.FirstOrDefault((CodeSnippet codeSnippet_0) => codeSnippet_0.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}
	}

	public void AddSnippet(CodeSnippet snippet)
	{
		lock (object_0)
		{
			snippet.Id = Guid.NewGuid().ToString();
			snippet.CreatedAt = DateTime.Now;
			snippet.LastUsed = DateTime.Now;
			snippet.UseCount = 0;
			codeSnippetLibrary_0.Snippets.Add(snippet);
			method_1();
			Logger.Info("[CodeSnippetStorage] 添加片段: " + snippet.Name);
		}
	}

	public void UpdateSnippet(CodeSnippet snippet)
	{
		lock (object_0)
		{
			CodeSnippet codeSnippet = codeSnippetLibrary_0.Snippets.FirstOrDefault((CodeSnippet codeSnippet_1) => codeSnippet_1.Id == snippet.Id);
			if (codeSnippet != null)
			{
				codeSnippet.Name = snippet.Name;
				codeSnippet.Description = snippet.Description;
				codeSnippet.Code = snippet.Code;
				codeSnippet.Tags = snippet.Tags;
				method_1();
				Logger.Info("[CodeSnippetStorage] 更新片段: " + snippet.Name);
			}
		}
	}

	public void DeleteSnippet(string id)
	{
		lock (object_0)
		{
			CodeSnippet codeSnippet = codeSnippetLibrary_0.Snippets.FirstOrDefault((CodeSnippet codeSnippet_0) => codeSnippet_0.Id == id);
			if (codeSnippet != null)
			{
				codeSnippetLibrary_0.Snippets.Remove(codeSnippet);
				method_1();
				Logger.Info("[CodeSnippetStorage] 删除片段: " + codeSnippet.Name);
			}
		}
	}

	public void RecordUsage(string id)
	{
		lock (object_0)
		{
			CodeSnippet codeSnippet = codeSnippetLibrary_0.Snippets.FirstOrDefault((CodeSnippet codeSnippet_0) => codeSnippet_0.Id == id);
			if (codeSnippet != null)
			{
				codeSnippet.LastUsed = DateTime.Now;
				codeSnippet.UseCount++;
				method_1();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippetStorage] 记录使用: ");
				defaultInterpolatedStringHandler.AppendFormatted(codeSnippet.Name);
				defaultInterpolatedStringHandler.AppendLiteral(" (总次数: ");
				defaultInterpolatedStringHandler.AppendFormatted(codeSnippet.UseCount);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
	}

	public List<CodeSnippet> SearchSnippets(string keyword)
	{
		lock (object_0)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return GetAllSnippets(sortByOrder: true);
			}
			string string_0 = keyword.ToLower();
			return (from codeSnippet_0 in codeSnippetLibrary_0.Snippets.Where((CodeSnippet codeSnippet_0) => codeSnippet_0.Name.ToLower().Contains(string_0) || codeSnippet_0.Description.ToLower().Contains(string_0) || codeSnippet_0.Tags.Any((string text) => text.ToLower().Contains(string_0)) || codeSnippet_0.Code.ToLower().Contains(string_0)).ToList()
				orderby (codeSnippet_0.Order != int.MaxValue) ? codeSnippet_0.Order : int.MaxValue, codeSnippet_0.LastUsed descending
				select codeSnippet_0).ToList();
		}
	}

	public List<CodeSnippet> FilterByTag(string tag)
	{
		lock (object_0)
		{
			return (from codeSnippet_0 in codeSnippetLibrary_0.Snippets.Where((CodeSnippet codeSnippet_0) => codeSnippet_0.Tags.Any((string string_1) => string_1.Equals(tag, StringComparison.OrdinalIgnoreCase))).ToList()
				orderby (codeSnippet_0.Order != int.MaxValue) ? codeSnippet_0.Order : int.MaxValue, codeSnippet_0.LastUsed descending
				select codeSnippet_0).ToList();
		}
	}

	public string ExportSnippet(string id)
	{
		lock (object_0)
		{
			CodeSnippet codeSnippet = codeSnippetLibrary_0.Snippets.FirstOrDefault((CodeSnippet codeSnippet_0) => codeSnippet_0.Id == id);
			if (codeSnippet == null)
			{
				throw new ArgumentException("未找到 ID 为 " + id + " 的代码片段");
			}
			return JsonConvert.SerializeObject((object)new CodeSnippetLibrary
			{
				Snippets = new List<CodeSnippet> { codeSnippet }
			}, (Formatting)1);
		}
	}

	public string ExportAllSnippets()
	{
		lock (object_0)
		{
			return JsonConvert.SerializeObject((object)codeSnippetLibrary_0, (Formatting)1);
		}
	}

	public void ImportSnippets(string json)
	{
		lock (object_0)
		{
			try
			{
				CodeSnippetLibrary codeSnippetLibrary = JsonConvert.DeserializeObject<CodeSnippetLibrary>(json);
				if (codeSnippetLibrary?.Snippets == null)
				{
					return;
				}
				int num = 0;
				foreach (CodeSnippet snippet in codeSnippetLibrary.Snippets)
				{
					snippet.Id = Guid.NewGuid().ToString();
					codeSnippetLibrary_0.Snippets.Add(snippet);
					num++;
				}
				method_1();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippetStorage] 导入成功，添加 ");
				defaultInterpolatedStringHandler.AppendFormatted(num);
				defaultInterpolatedStringHandler.AppendLiteral(" 个代码片段");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			catch (Exception ex)
			{
				Logger.Error("[CodeSnippetStorage] 导入失败: " + ex.Message, ex);
				throw;
			}
		}
	}

	public List<string> GetAllTags()
	{
		lock (object_0)
		{
			return (from string_0 in codeSnippetLibrary_0.Snippets.SelectMany((CodeSnippet codeSnippet_0) => codeSnippet_0.Tags).Distinct()
				orderby string_0
				select string_0).ToList();
		}
	}

	public void UpdateSnippetOrder(string id, int newOrder)
	{
		lock (object_0)
		{
			CodeSnippet codeSnippet = codeSnippetLibrary_0.Snippets.FirstOrDefault((CodeSnippet codeSnippet_0) => codeSnippet_0.Id == id);
			if (codeSnippet != null)
			{
				codeSnippet.Order = newOrder;
				method_1();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippetStorage] 更新排序: ");
				defaultInterpolatedStringHandler.AppendFormatted(codeSnippet.Name);
				defaultInterpolatedStringHandler.AppendLiteral(" -> Order=");
				defaultInterpolatedStringHandler.AppendFormatted(newOrder);
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
	}

	public void MoveSnippetUp(string id, bool sortByOrder)
	{
		lock (object_0)
		{
			method_4();
			List<CodeSnippet> list = codeSnippetLibrary_0.Snippets.OrderBy((CodeSnippet codeSnippet_0) => codeSnippet_0.Order).ToList();
			int num = list.FindIndex((CodeSnippet codeSnippet_0) => codeSnippet_0.Id == id);
			if (num > 0)
			{
				CodeSnippet codeSnippet = list[num];
				CodeSnippet codeSnippet2 = list[num - 1];
				if (codeSnippet != null && codeSnippet2 != null)
				{
					int order = codeSnippet.Order;
					codeSnippet.Order = codeSnippet2.Order;
					codeSnippet2.Order = order;
					method_1();
					Logger.Info("[CodeSnippetStorage] 上移片段: " + codeSnippet.Name);
				}
			}
		}
	}

	public void MoveSnippetDown(string id, bool sortByOrder)
	{
		lock (object_0)
		{
			method_4();
			List<CodeSnippet> list = codeSnippetLibrary_0.Snippets.OrderBy((CodeSnippet codeSnippet_0) => codeSnippet_0.Order).ToList();
			int num = list.FindIndex((CodeSnippet codeSnippet_0) => codeSnippet_0.Id == id);
			if (num >= 0 && num < list.Count - 1)
			{
				CodeSnippet codeSnippet = list[num];
				CodeSnippet codeSnippet2 = list[num + 1];
				if (codeSnippet != null && codeSnippet2 != null)
				{
					int order = codeSnippet.Order;
					codeSnippet.Order = codeSnippet2.Order;
					codeSnippet2.Order = order;
					method_1();
					Logger.Info("[CodeSnippetStorage] 下移片段: " + codeSnippet.Name);
				}
			}
		}
	}

	private void method_3()
	{
		List<CodeSnippet> list = codeSnippetLibrary_0.Snippets.OrderByDescending((CodeSnippet codeSnippet_0) => codeSnippet_0.LastUsed).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			list[num].Order = num;
		}
		method_1();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippetStorage] 初始化排序顺序，共 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个片段");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
	}

	private void method_4()
	{
		if (codeSnippetLibrary_0.Snippets.Any((CodeSnippet codeSnippet_0) => codeSnippet_0.Order == int.MaxValue))
		{
			method_3();
		}
	}

	public void ReorderSnippets()
	{
		lock (object_0)
		{
			List<CodeSnippet> list = (from codeSnippet_0 in codeSnippetLibrary_0.Snippets
				where codeSnippet_0.Order != int.MaxValue
				orderby codeSnippet_0.Order
				select codeSnippet_0).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				list[num].Order = num;
			}
			method_1();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CodeSnippetStorage] 整理排序完成，共 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个片段");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
