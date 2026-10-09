using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using ns6;

namespace RevitAi.Revit.Services;

public class FamilyLoadService : IFamilyLoadService
{
	private readonly IRevitAdapter _revitAdapter;

	public FamilyLoadService(IRevitAdapter revitAdapter)
	{
		_revitAdapter = revitAdapter ?? throw new ArgumentNullException("revitAdapter");
	}

	public Task<bool> LoadFamilyAsync(string filePath, string familyName)
	{
		return Task.FromResult(LoadFamily(filePath, familyName));
	}

	private bool LoadFamily(string filePath, string familyName)
	{
		try
		{
			object activeDocument = _revitAdapter.GetActiveDocument();
			Document val = (Document)((activeDocument is Document) ? activeDocument : null);
			if (val != null)
			{
				string text = string.Join("_", familyName.Split(Path.GetInvalidFileNameChars()));
				if (string.IsNullOrWhiteSpace(text))
				{
					text = Guid.NewGuid().ToString("N");
				}
				string text2 = Path.Combine(Path.GetTempPath(), text + ".rfa");
				try
				{
					if (!File.Exists(filePath))
					{
						Logger.Error("[FamilyLoadService] 族文件不存在: " + filePath);
						return false;
					}
					File.Copy(filePath, text2, overwrite: true);
					FamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();
					Family val2 = null;
					if (val.LoadFamily(text2, (IFamilyLoadOptions)(object)familyLoadOptions, out val2) && val2 != null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[FamilyLoadService] 族加载成功: ");
						defaultInterpolatedStringHandler.AppendFormatted(familyName);
						defaultInterpolatedStringHandler.AppendLiteral(" (ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Id.Value);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						return true;
					}
					Logger.Warning("[FamilyLoadService] 族加载失败: " + familyName);
					return false;
				}
				finally
				{
					if (File.Exists(text2))
					{
						try
						{
							File.Delete(text2);
						}
						catch (Exception ex)
						{
							Logger.Debug("[FamilyLoadService] 清理临时文件失败: " + ex.Message);
						}
					}
				}
			}
			Logger.Warning("[FamilyLoadService] 无法获取活动文档");
			return false;
		}
		catch (Exception ex2)
		{
			Logger.Error("[FamilyLoadService] 加载族异常: " + familyName, ex2);
			return false;
		}
	}

	public Task<bool> FamilyExistsAsync(string familyName)
	{
		return Task.Run(delegate
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			try
			{
				object activeDocument = _revitAdapter.GetActiveDocument();
				if (activeDocument == null)
				{
					return false;
				}
				FilteredElementCollector val = new FilteredElementCollector((Document)((activeDocument is Document) ? activeDocument : null));
				val.OfClass(typeof(Family));
				foreach (Element item in val)
				{
					Family val2 = (Family)(object)((item is Family) ? item : null);
					if (val2 != null && ((Element)val2).Name.Equals(familyName, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				Logger.Warning("[FamilyLoadService] 检查族存在性异常: " + familyName + " - " + ex.Message);
				return false;
			}
		});
	}
}
