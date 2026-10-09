using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ns6;

namespace RevitAi.Revit.Net8.AI.Security;

public static class CodeSandboxValidator
{
	private static readonly string[] string_0 = new string[12]
	{
		"System.IO",
		"System.Net",
		"System.Diagnostics.Process",
		"Microsoft.Win32",
		"System.Runtime.InteropServices",
		"System.Reflection",
		"Microsoft.CodeAnalysis",
		"System.AppDomain",
		"System.Threading.Thread",
		"System.Configuration",
		"System.Security",
		"System.Security.Cryptography"
	};

	private static readonly HashSet<string> hashSet_0 = new HashSet<string>
	{
		"File",
		"Directory",
		"Path",
		"FileStream",
		"StreamReader",
		"StreamWriter",
		"FileSystem",
		"FileSystemWatcher",
		"DriveInfo",
		"Process",
		"ProcessStartInfo",
		"Registry",
		"RegistryKey",
		"WebClient",
		"HttpClient",
		"HttpWebRequest",
		"Socket",
		"TcpClient",
		"TcpListener",
		"Assembly",
		"AppDomain",
		"MethodInfo",
		"TypeInfo",
		"Environment"
	};

	private static readonly string[] string_1 = new string[17]
	{
		"\\.LoadFrom\\s*\\(",
		"\\.LoadFile\\s*\\(",
		"\\.LoadModule\\s*\\(",
		"\\.Kill\\s*\\(",
		"\\.WriteAllText\\s*\\(",
		"\\.WriteAllBytes\\s*\\(",
		"\\.WriteAllLines\\s*\\(",
		"\\.ReadAllText\\s*\\(",
		"\\.ReadAllBytes\\s*\\(",
		"\\.ReadAllLines\\s*\\(",
		"\\.OpenSubKey\\s*\\(",
		"\\.InvokeMember\\s*\\(",
		"\\.GetTypeInfo\\s*\\(",
		"\\.GetExecutingAssembly\\s*\\(",
		"\\.CreateInstance\\s*\\(",
		"\\.SetEnvironmentVariable\\s*\\(",
		"\\.GetEnvironmentVariable\\s*\\("
	};

	public static ValidationResult Validate(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
		{
			return ValidationResult.Invalid("代码为空", "SyntaxError");
		}
		try
		{
			SyntaxTree val = CSharpSyntaxTree.ParseText(code, (CSharpParseOptions)null, "", (Encoding)null, default(CancellationToken));
			SyntaxNode root = val.GetRoot(default(CancellationToken));
			ValidationResult validationResult = smethod_0(root);
			if (!validationResult.IsValid)
			{
				return validationResult;
			}
			ValidationResult validationResult2 = smethod_1(root);
			if (!validationResult2.IsValid)
			{
				return validationResult2;
			}
			ValidationResult validationResult3 = smethod_2(root);
			if (!validationResult3.IsValid)
			{
				return validationResult3;
			}
			ValidationResult validationResult4 = smethod_3(root);
			if (!validationResult4.IsValid)
			{
				return validationResult4;
			}
			return ValidationResult.Valid();
		}
		catch (Exception ex)
		{
			return ValidationResult.Invalid("语法分析异常: " + ex.Message, "SyntaxError");
		}
	}

	private static ValidationResult smethod_0(SyntaxNode syntaxNode_0)
	{
		IEnumerable<UsingDirectiveSyntax> enumerable = syntaxNode_0.DescendantNodes((Func<SyntaxNode, bool>)null, false).OfType<UsingDirectiveSyntax>();
		foreach (UsingDirectiveSyntax item in enumerable)
		{
			NameSyntax name = item.Name;
			object obj;
			if (name == null)
			{
				obj = null;
			}
			else
			{
				obj = ((object)name).ToString();
				if (obj != null)
				{
					goto IL_003a;
				}
			}
			obj = string.Empty;
			goto IL_003a;
			IL_003a:
			string text = (string)obj;
			string[] array = string_0;
			foreach (string value in array)
			{
				if (text.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					return ValidationResult.Invalid("禁止使用命名空间 '" + text + "'（安全限制：禁止文件/进程/网络/反射等操作）", "SecurityViolation", smethod_4((SyntaxNode)(object)item), text);
				}
			}
		}
		return ValidationResult.Valid();
	}

	private static ValidationResult smethod_1(SyntaxNode syntaxNode_0)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<IdentifierNameSyntax> enumerable = syntaxNode_0.DescendantNodes((Func<SyntaxNode, bool>)null, false).OfType<IdentifierNameSyntax>();
		foreach (IdentifierNameSyntax item in enumerable)
		{
			SyntaxToken identifier = ((SimpleNameSyntax)item).Identifier;
			string valueText = ((SyntaxToken)identifier).ValueText;
			if (hashSet_0.Contains(valueText))
			{
				SyntaxNode parent = ((SyntaxNode)item).Parent;
				if (parent is ObjectCreationExpressionSyntax || parent is MemberAccessExpressionSyntax || parent is QualifiedNameSyntax || parent is ArgumentSyntax)
				{
					return ValidationResult.Invalid("禁止使用类型 '" + valueText + "'（安全限制）", "SecurityViolation", smethod_4((SyntaxNode)(object)item), valueText);
				}
			}
		}
		return ValidationResult.Valid();
	}

	private static ValidationResult smethod_2(SyntaxNode syntaxNode_0)
	{
		IEnumerable<MemberAccessExpressionSyntax> enumerable = syntaxNode_0.DescendantNodes((Func<SyntaxNode, bool>)null, false).OfType<MemberAccessExpressionSyntax>();
		foreach (MemberAccessExpressionSyntax item in enumerable)
		{
			string text = ((object)item).ToString();
			string[] array = string_1;
			foreach (string pattern in array)
			{
				if (Regex.IsMatch(text, pattern))
				{
					return ValidationResult.Invalid("禁止的成员访问: '" + text + "'（安全限制）", "SecurityViolation", smethod_4((SyntaxNode)(object)item), text);
				}
			}
		}
		return ValidationResult.Valid();
	}

	private static ValidationResult smethod_3(SyntaxNode syntaxNode_0)
	{
		List<GotoStatementSyntax> source = syntaxNode_0.DescendantNodes((Func<SyntaxNode, bool>)null, false).OfType<GotoStatementSyntax>().ToList();
		if (source.Any())
		{
			return ValidationResult.Invalid("禁止使用 goto 语句（安全限制）", "SecurityViolation", smethod_4((SyntaxNode)(object)source.First()), "goto");
		}
		IEnumerable<WhileStatementSyntax> enumerable = syntaxNode_0.DescendantNodes((Func<SyntaxNode, bool>)null, false).OfType<WhileStatementSyntax>();
		foreach (WhileStatementSyntax item in enumerable)
		{
			ExpressionSyntax condition = item.Condition;
			LiteralExpressionSyntax val = (LiteralExpressionSyntax)(object)((condition is LiteralExpressionSyntax) ? condition : null);
			if (val != null && Microsoft.CodeAnalysis.CSharpExtensions.IsKind((SyntaxNode)(object)val, (SyntaxKind)8752))
			{
				return ValidationResult.Invalid("禁止 while(true) 无限循环（安全限制）", "SecurityViolation", smethod_4((SyntaxNode)(object)item), "while(true)");
			}
		}
		IEnumerable<ForStatementSyntax> enumerable2 = syntaxNode_0.DescendantNodes((Func<SyntaxNode, bool>)null, false).OfType<ForStatementSyntax>();
		foreach (ForStatementSyntax item2 in enumerable2)
		{
			if (item2.Condition == null)
			{
				return ValidationResult.Invalid("禁止 for(;;) 无限循环（安全限制）", "SecurityViolation", smethod_4((SyntaxNode)(object)item2), "for(;;)");
			}
		}
		return ValidationResult.Valid();
	}

	private static int smethod_4(SyntaxNode syntaxNode_0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		FileLinePositionSpan lineSpan = syntaxNode_0.GetLocation().GetLineSpan();
		LinePosition startLinePosition = ((FileLinePositionSpan)lineSpan).StartLinePosition;
		return ((LinePosition)startLinePosition).Line + 1;
	}
}
