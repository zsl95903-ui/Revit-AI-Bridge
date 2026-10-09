using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns1;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class LinkService : ILinkService
{
	private readonly UIApplication _application;

	public LinkService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public IEnumerable<object> GetAllLinks(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(RevitLinkType));
			return val2.ToElements();
		}
		catch (Exception ex)
		{
			Logger.Error("GetAllLinks 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetAllLinkInstances(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfCategory((BuiltInCategory)(-2001352L)).WhereElementIsNotElementType().OfClass(typeof(RevitLinkInstance));
			return val2.ToElements();
		}
		catch (Exception ex)
		{
			Logger.Error("GetAllLinkInstances 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetLinks(object document)
	{
		return GetAllLinks(document);
	}

	public object? LoadLink(object document, string filePath, string? linkTypeName = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			ModelPath val2 = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
			RevitLinkOptions val3 = new RevitLinkOptions(false);
			LinkLoadResult val4 = RevitLinkType.Create(val, val2, val3);
			if (val4 == null)
			{
				Logger.Error("LoadLink: 无法创建链接 " + filePath);
				return null;
			}
			return RevitLinkInstance.Create(val, val4.ElementId);
		}
		catch (Exception ex)
		{
			Logger.Error("LoadLink 失败: " + ex.Message);
			return null;
		}
	}

	public int LoadLinks(object document, IEnumerable<string> filePaths)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return 0;
			}
			int num = 0;
			Transaction val2 = new Transaction(val, "批量链接模型");
			try
			{
				val2.Start();
				foreach (string filePath in filePaths)
				{
					try
					{
						ModelPath val3 = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
						RevitLinkOptions val4 = new RevitLinkOptions(false);
						LinkLoadResult val5 = RevitLinkType.Create(val, val3, val4);
						if (val5 != null)
						{
							RevitLinkInstance.Create(val, val5.ElementId);
							num++;
						}
					}
					catch (Exception ex)
					{
						Logger.Error("加载链接失败 " + filePath + ": " + ex.Message);
					}
				}
				val2.Commit();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			return num;
		}
		catch (Exception ex2)
		{
			Logger.Error("LoadLinks 失败: " + ex2.Message);
			return 0;
		}
	}

	public object? LoadLink(object document, string filePath, int linkTypeId, object? position = null)
	{
		return LoadLink(document, filePath);
	}

	public bool UnloadLink(object document, int linkId)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return false;
			}
			ElementId val2 = new ElementId((long)linkId);
			Element element = val.GetElement(val2);
			RevitLinkType val3 = (RevitLinkType)(object)((element is RevitLinkType) ? element : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
				defaultInterpolatedStringHandler.AppendLiteral("UnloadLink: 找不到链接 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(linkId);
				defaultInterpolatedStringHandler.AppendLiteral(" (ElementId: ");
				defaultInterpolatedStringHandler.AppendFormatted(val2.Value);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			val3.Unload((ISaveSharedCoordinatesCallback)null);
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("UnloadLink 失败: " + ex.Message);
			return false;
		}
	}

	public unsafe bool ReloadLink(object document, int linkId)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Expected O, but got Unknown
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Invalid comparison between Unknown and I4
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Invalid comparison between Unknown and I4
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("ReloadLink: 文档对象为空或类型错误");
				return false;
			}
			ElementId val2 = new ElementId((long)linkId);
			Element element = val.GetElement(val2);
			RevitLinkType val3 = (RevitLinkType)(object)((element is RevitLinkType) ? element : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
				defaultInterpolatedStringHandler.AppendLiteral("ReloadLink: 找不到链接 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(linkId);
				defaultInterpolatedStringHandler.AppendLiteral(" (ElementId: ");
				defaultInterpolatedStringHandler.AppendFormatted(val2.Value);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			bool flag = RevitLinkType.IsLoaded(val, ((Element)val3).Id);
			ExternalFileReference externalFileReference = ((Element)val3).GetExternalFileReference();
			if (externalFileReference == null)
			{
				Logger.Error("ReloadLink: 无法获取外部文件引用 " + ((Element)val3).Name);
				return false;
			}
			string text = ModelPathUtils.ConvertModelPathToUserVisiblePath(externalFileReference.GetPath());
			string text2 = text;
			if (string.IsNullOrEmpty(text) || text.Length < 2 || (text[1] != ':' && !text.StartsWith("\\\\")))
			{
				string pathName = val.PathName;
				if (!string.IsNullOrEmpty(pathName))
				{
					string directoryName = Path.GetDirectoryName(pathName);
					if (!string.IsNullOrEmpty(directoryName))
					{
						text2 = Path.Combine(directoryName, text);
						text2 = Path.GetFullPath(text2);
						Logger.Error("ReloadLink: Converted relative to absolute: " + text + " -> " + text2);
					}
				}
			}
			ModelPath val4 = ModelPathUtils.ConvertUserVisiblePathToModelPath(text2);
			bool result = false;
			if (flag)
			{
				LinkLoadResult val5 = val3.Reload();
				PropertyInfo property = ((object)val5).GetType().GetProperty("Result");
				if (property != null)
				{
					object value = property.GetValue(val5);
					Logger.Error("ReloadLink: Result value: " + value);
					if (value is LinkLoadResultType val6)
					{
						result = (int)val6 == 1;
						Logger.Error("ReloadLink: Result type: " + ((object)(*(LinkLoadResultType*)(&val6))/*cast due to constrained. prefix*/).ToString() + " success=" + result);
					}
				}
				else
				{
					result = true;
				}
				Logger.Error("ReloadLink: Reload returned " + ((object)val5).ToString() + " success=" + result);
			}
			else
			{
				LinkLoadResult val7 = val3.LoadFrom(val4, new WorksetConfiguration());
				PropertyInfo property2 = ((object)val7).GetType().GetProperty("Result");
				if (property2 != null)
				{
					if (property2.GetValue(val7) is LinkLoadResultType val8)
					{
						result = (int)val8 == 1;
						Logger.Error("ReloadLink: Result type: " + ((object)(*(LinkLoadResultType*)(&val8))/*cast due to constrained. prefix*/).ToString() + " success=" + result);
					}
				}
				else
				{
					Logger.Error("ReloadLink: No Result property, checking if LinkLoadResult is enum-compatible");
					result = true;
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			Logger.Error("ReloadLink 失败: " + ex.Message + "\n" + ex.StackTrace);
			return false;
		}
	}

	public (string? FilePath, string? LinkType, bool IsLoaded, int? LinkTypeId)? GetLinkInfo(object linkElement)
	{
		try
		{
			RevitLinkType val = (RevitLinkType)((linkElement is RevitLinkType) ? linkElement : null);
			if (val == null)
			{
				return null;
			}
			ExternalFileReference externalFileReference = ((Element)val).GetExternalFileReference();
			string item = ModelPathUtils.ConvertModelPathToUserVisiblePath(externalFileReference.GetPath());
			bool item2 = IsLinkLoaded(val);
			return (item, ((Element)val).Name, item2, ((Element)val).Id.smethod_0());
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkInfo 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetLinkName(object linkElement)
	{
		try
		{
			RevitLinkType val = (RevitLinkType)((linkElement is RevitLinkType) ? linkElement : null);
			if (val == null)
			{
				return null;
			}
			return ((Element)val).Name;
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkName 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetLinkPath(object linkElement)
	{
		try
		{
			RevitLinkType val = (RevitLinkType)((linkElement is RevitLinkType) ? linkElement : null);
			if (val == null)
			{
				return null;
			}
			ExternalFileReference externalFileReference = ((Element)val).GetExternalFileReference();
			if (externalFileReference == null)
			{
				return null;
			}
			return ModelPathUtils.ConvertModelPathToUserVisiblePath(externalFileReference.GetPath());
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkPath 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetLinkType(object linkElement)
	{
		try
		{
			RevitLinkType val = (RevitLinkType)((linkElement is RevitLinkType) ? linkElement : null);
			if (val == null)
			{
				return null;
			}
			return ((Element)val).Name;
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkType 失败: " + ex.Message);
			return null;
		}
	}

	public bool IsLinkLoaded(object linkElement)
	{
		try
		{
			RevitLinkType val = (RevitLinkType)((linkElement is RevitLinkType) ? linkElement : null);
			if (val == null)
			{
				return false;
			}
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			Document val2 = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val2 == null)
			{
				return false;
			}
			return RevitLinkType.IsLoaded(val2, ((Element)val).Id);
		}
		catch (Exception ex)
		{
			Logger.Error("IsLinkLoaded 失败: " + ex.Message);
			return false;
		}
	}

	public (double X, double Y, double Z)? GetLinkPosition(object linkElement)
	{
		try
		{
			RevitLinkInstance val = (RevitLinkInstance)((linkElement is RevitLinkInstance) ? linkElement : null);
			if (val == null)
			{
				return null;
			}
			Transform totalTransform = ((Instance)val).GetTotalTransform();
			XYZ origin = totalTransform.Origin;
			return (UnitUtils.ConvertFromInternalUnits(origin.X, UnitTypeId.Meters), UnitUtils.ConvertFromInternalUnits(origin.Y, UnitTypeId.Meters), UnitUtils.ConvertFromInternalUnits(origin.Z, UnitTypeId.Meters));
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkPosition 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetLinkRotation(object linkElement)
	{
		try
		{
			RevitLinkInstance val = (RevitLinkInstance)((linkElement is RevitLinkInstance) ? linkElement : null);
			if (val == null)
			{
				return null;
			}
			Transform totalTransform = ((Instance)val).GetTotalTransform();
			XYZ basisX = totalTransform.BasisX;
			double num = 0.0 - Math.Atan2(basisX.Y, basisX.X);
			return num * 180.0 / Math.PI;
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkRotation 失败: " + ex.Message);
			return null;
		}
	}

	public bool UpdateLinkPosition(object document, object linkElement, double x, double y, double z)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val != null)
			{
				RevitLinkInstance val2 = (RevitLinkInstance)((linkElement is RevitLinkInstance) ? linkElement : null);
				if (val2 != null)
				{
					Transform totalTransform = ((Instance)val2).GetTotalTransform();
					XYZ origin = totalTransform.Origin;
					XYZ val3 = new XYZ(UnitUtils.ConvertToInternalUnits(x, UnitTypeId.Meters), UnitUtils.ConvertToInternalUnits(y, UnitTypeId.Meters), UnitUtils.ConvertToInternalUnits(z, UnitTypeId.Meters));
					XYZ val4 = val3 - origin;
					((Element)val2).Location.Move(val4);
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("UpdateLinkPosition 失败: " + ex.Message);
			return false;
		}
	}

	public bool UpdateLinkRotation(object document, object linkElement, double rotationAngle)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val != null)
			{
				RevitLinkInstance val2 = (RevitLinkInstance)((linkElement is RevitLinkInstance) ? linkElement : null);
				if (val2 != null)
				{
					double? linkRotation = GetLinkRotation(val2);
					if (!linkRotation.HasValue)
					{
						return false;
					}
					Transform totalTransform = ((Instance)val2).GetTotalTransform();
					XYZ origin = totalTransform.Origin;
					Line val3 = Line.CreateBound(origin, origin + XYZ.BasisZ);
					double num = (rotationAngle - linkRotation.Value) * Math.PI / 180.0;
					((Element)val2).Location.Rotate(val3, num);
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("UpdateLinkRotation 失败: " + ex.Message);
			return false;
		}
	}

	public bool DeleteLink(object document, object linkElement)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Invalid comparison between Unknown and I4
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Invalid comparison between Unknown and I4
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val != null)
			{
				RevitLinkInstance val2 = (RevitLinkInstance)((linkElement is RevitLinkInstance) ? linkElement : null);
				if (val2 != null)
				{
					ElementId typeId = ((Element)val2).GetTypeId();
					Element element = val.GetElement(typeId);
					RevitLinkType val3 = (RevitLinkType)(object)((element is RevitLinkType) ? element : null);
					if (val3 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
						defaultInterpolatedStringHandler.AppendLiteral("DeleteLink: 找不到链接类型，类型 ID=");
						defaultInterpolatedStringHandler.AppendFormatted<ElementId>(typeId);
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
						return false;
					}
					Transaction val4 = new Transaction(val, "删除链接");
					try
					{
						TransactionStatus val5 = val4.Start();
						if ((int)val5 != 1)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("DeleteLink: Transaction 启动失败，状态=");
							defaultInterpolatedStringHandler2.AppendFormatted<TransactionStatus>(val5);
							Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
							return false;
						}
						val.Delete(((Element)val2).Id);
						val.Delete(((Element)val3).Id);
						TransactionStatus val6 = val4.Commit();
						if ((int)val6 != 3)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(32, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("DeleteLink: Transaction 提交失败，状态=");
							defaultInterpolatedStringHandler3.AppendFormatted<TransactionStatus>(val6);
							Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
							return false;
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
					return true;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("DeleteLink: 参数类型错误 (document=");
			defaultInterpolatedStringHandler4.AppendFormatted(document != null);
			defaultInterpolatedStringHandler4.AppendLiteral(", linkElement=");
			defaultInterpolatedStringHandler4.AppendFormatted(linkElement != null);
			defaultInterpolatedStringHandler4.AppendLiteral(")");
			Logger.Error(defaultInterpolatedStringHandler4.ToStringAndClear());
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("DeleteLink 失败: " + ex.Message + "\n" + ex.StackTrace);
			return false;
		}
	}

	public object? GetLinkTypeFromInstance(object linkInstance)
	{
		try
		{
			RevitLinkInstance val = (RevitLinkInstance)((linkInstance is RevitLinkInstance) ? linkInstance : null);
			if (val == null)
			{
				return null;
			}
			ElementId typeId = ((Element)val).GetTypeId();
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			Document val2 = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val2 == null)
			{
				Logger.Error("GetLinkTypeFromInstance: 无法获取活动文档");
				return null;
			}
			return val2.GetElement(typeId);
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkTypeFromInstance 失败: " + ex.Message);
			return null;
		}
	}

	public bool ReplaceLinkPath(object document, object linkInstance, string newFilePath)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val != null)
			{
				RevitLinkInstance val2 = (RevitLinkInstance)((linkInstance is RevitLinkInstance) ? linkInstance : null);
				if (val2 != null)
				{
					Transform totalTransform = ((Instance)val2).GetTotalTransform();
					XYZ origin = totalTransform.Origin;
					XYZ basisX = totalTransform.BasisX;
					double num = (0.0 - Math.Atan2(basisX.Y, basisX.X)) * 180.0 / Math.PI;
					ElementId id = ((Element)val2).Id;
					Transaction val3 = new Transaction(val, "替换链接路径");
					try
					{
						val3.Start();
						val.Delete(id);
						ModelPath val4 = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFilePath);
						RevitLinkOptions val5 = new RevitLinkOptions(false);
						LinkLoadResult val6 = RevitLinkType.Create(val, val4, val5);
						if (val6 == null)
						{
							Logger.Error("ReplaceLinkPath: 无法创建新链接类型 " + newFilePath);
							val3.RollBack();
							return false;
						}
						RevitLinkInstance val7 = RevitLinkInstance.Create(val, val6.ElementId);
						if (val7 == null)
						{
							Logger.Error("ReplaceLinkPath: 无法创建新链接实例");
							val3.RollBack();
							return false;
						}
						Transform totalTransform2 = ((Instance)val7).GetTotalTransform();
						XYZ origin2 = totalTransform2.Origin;
						XYZ val8 = origin - origin2;
						if (!origin2.IsAlmostEqualTo(origin))
						{
							((Element)val7).Location.Move(val8);
						}
						XYZ basisX2 = ((Instance)val7).GetTotalTransform().BasisX;
						double num2 = (0.0 - Math.Atan2(basisX2.Y, basisX2.X)) * 180.0 / Math.PI;
						double num3 = (num - num2) * Math.PI / 180.0;
						if (Math.Abs(num3) > 0.001)
						{
							Line val9 = Line.CreateBound(origin, origin + XYZ.BasisZ);
							((Element)val7).Location.Rotate(val9, num3);
						}
						val3.Commit();
						return true;
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
			}
			Logger.Error("ReplaceLinkPath: 参数类型错误");
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("ReplaceLinkPath 失败: " + ex.Message);
			return false;
		}
	}

	public object? GetLinkDocument(object linkInstance)
	{
		try
		{
			RevitLinkInstance val = (RevitLinkInstance)((linkInstance is RevitLinkInstance) ? linkInstance : null);
			if (val == null)
			{
				Logger.Error("GetLinkDocument: 参数类型错误，需要 RevitLinkInstance");
				return null;
			}
			Document linkDocument = val.GetLinkDocument();
			if (linkDocument == null)
			{
				Logger.Error("GetLinkDocument: 无法获取链接文档，链接可能未加载");
				return null;
			}
			return linkDocument;
		}
		catch (Exception ex)
		{
			Logger.Error("GetLinkDocument 失败: " + ex.Message);
			return null;
		}
	}
}
