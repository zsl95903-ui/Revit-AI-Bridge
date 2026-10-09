using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ns6;

using OperationCanceledException = System.OperationCanceledException;
using ArgumentNullException = System.ArgumentNullException;
namespace RevitAi.Revit.Services;

internal sealed class InsulationService : IInsulationService
{
	private readonly UIApplication _application;

	private static PropertyInfo? _pipeHostElementIdProperty;

	private static PropertyInfo? _pipeIdProperty;

	private static PropertyInfo? _ductHostElementIdProperty;

	private static PropertyInfo? _ductIdProperty;

	private Document? ActiveDocument
	{
		get
		{
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			return (activeUIDocument != null) ? activeUIDocument.Document : null;
		}
	}

	private UIDocument? ActiveUIDocument => _application.ActiveUIDocument;

	public InsulationService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	private static int ToInt(ElementId id)
	{
		return (int)id.Value;
	}

	private static ElementId ToId(int value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		return new ElementId((long)value);
	}

	public List<SystemInsulationInfo> LoadAllSystemData(object document)
	{
		List<SystemInsulationInfo> result = new List<SystemInsulationInfo>();
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			Logger.Error("[InsulationService] LoadAllSystemData: document 参数无效");
			return result;
		}
		LoadPipeSystemData(val, result);
		LoadDuctSystemData(val, result);
		return result;
	}

	public List<InsulationTypeInfoLite> LoadAllInsulationTypes(object document)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		List<InsulationTypeInfoLite> list = new List<InsulationTypeInfoLite>();
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return list;
		}
		foreach (PipeInsulationType item in ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(PipeInsulationType))).Cast<PipeInsulationType>())
		{
			list.Add(new InsulationTypeInfoLite
			{
				Id = ToInt(((Element)item).Id),
				Name = ((Element)item).Name,
				ElementType = (SystemElementType)0
			});
		}
		foreach (DuctInsulationType item2 in ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(DuctInsulationType))).Cast<DuctInsulationType>())
		{
			list.Add(new InsulationTypeInfoLite
			{
				Id = ToInt(((Element)item2).Id),
				Name = ((Element)item2).Name,
				ElementType = (SystemElementType)1
			});
		}
		return list;
	}

	private void LoadPipeSystemData(Document doc, List<SystemInsulationInfo> result)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Expected O, but got Unknown
		List<Pipe> source = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Pipe))).Cast<Pipe>().ToList();
		List<Element> fittings = CollectPipeFittings(doc);
		Dictionary<ElementId, PipeInsulation> insulationDict = new Dictionary<ElementId, PipeInsulation>();
		foreach (PipeInsulation item in ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>())
		{
			try
			{
				ElementId hostElementIdForPipeInsulation = GetHostElementIdForPipeInsulation(item);
				if (hostElementIdForPipeInsulation != (ElementId)null && hostElementIdForPipeInsulation != ElementId.InvalidElementId && !insulationDict.ContainsKey(hostElementIdForPipeInsulation))
				{
					insulationDict[hostElementIdForPipeInsulation] = item;
				}
			}
			catch
			{
			}
		}
		foreach (IGrouping<int, Pipe> item2 in from p in source
			group p by GetPipeSystemTypeId(doc, p))
		{
			if (item2.Key < 0)
			{
				continue;
			}
			try
			{
				Element element = doc.GetElement(ToId(item2.Key));
				PipingSystemType val = (PipingSystemType)(object)((element is PipingSystemType) ? element : null);
				if (val == null)
				{
					continue;
				}
				List<Element> list = FilterFittingsBySystem(doc, fittings, (BuiltInParameter)(-1140334L), item2.Key);
				int elementCount = item2.Count() + list.Count;
				int num = list.Count((Element f) => insulationDict.ContainsKey(f.Id)) + item2.Count((Pipe p) => insulationDict.ContainsKey(((Element)p).Id));
				string thicknessInput = "0";
				string materialInput = "";
				if (num > 0)
				{
					HashSet<ElementId> systemHostIds = new HashSet<ElementId>(item2.Select((Pipe p) => ((Element)p).Id));
					foreach (Element item3 in list)
					{
						systemHostIds.Add(item3.Id);
					}
					List<Element> insulations = (from kv in insulationDict
						where systemHostIds.Contains(kv.Key)
						select kv.Value).Cast<Element>().ToList();
					Tuple<string, string> dominantInsulationValues = GetDominantInsulationValues(doc, insulations);
					if (dominantInsulationValues != null)
					{
						thicknessInput = dominantInsulationValues.Item1;
						materialInput = dominantInsulationValues.Item2;
					}
				}
				result.Add(new SystemInsulationInfo
				{
					SystemTypeId = ToInt(((Element)val).Id),
					SystemTypeName = ((Element)val).Name,
					ElementType = (SystemElementType)0,
					ElementCount = elementCount,
					InsulatedCount = num,
					ThicknessInput = thicknessInput,
					MaterialInput = materialInput
				});
			}
			catch
			{
			}
		}
	}

	private void LoadDuctSystemData(Document doc, List<SystemInsulationInfo> result)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Expected O, but got Unknown
		List<Duct> source = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Duct))).Cast<Duct>().ToList();
		List<Element> fittings = CollectDuctFittings(doc);
		Dictionary<ElementId, DuctInsulation> insulationDict = new Dictionary<ElementId, DuctInsulation>();
		foreach (DuctInsulation item in ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>())
		{
			try
			{
				ElementId hostElementIdForDuctInsulation = GetHostElementIdForDuctInsulation(item);
				if (hostElementIdForDuctInsulation != (ElementId)null && hostElementIdForDuctInsulation != ElementId.InvalidElementId && !insulationDict.ContainsKey(hostElementIdForDuctInsulation))
				{
					insulationDict[hostElementIdForDuctInsulation] = item;
				}
			}
			catch
			{
			}
		}
		foreach (IGrouping<int, Duct> item2 in from d in source
			group d by GetDuctSystemTypeId(d))
		{
			if (item2.Key < 0)
			{
				continue;
			}
			try
			{
				Element element = doc.GetElement(ToId(item2.Key));
				if (element == null)
				{
					continue;
				}
				string text = element.Name;
				if (string.IsNullOrEmpty(text))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
					defaultInterpolatedStringHandler.AppendLiteral("风管系统 ");
					defaultInterpolatedStringHandler.AppendFormatted(item2.Key);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				List<Element> list = FilterFittingsBySystem(doc, fittings, (BuiltInParameter)(-1140333L), item2.Key);
				int elementCount = item2.Count() + list.Count;
				int num = list.Count((Element f) => insulationDict.ContainsKey(f.Id)) + item2.Count((Duct d) => insulationDict.ContainsKey(((Element)d).Id));
				string thicknessInput = "0";
				string materialInput = "";
				if (num > 0)
				{
					HashSet<ElementId> systemHostIds = new HashSet<ElementId>(item2.Select((Duct d) => ((Element)d).Id));
					foreach (Element item3 in list)
					{
						systemHostIds.Add(item3.Id);
					}
					List<Element> insulations = (from kv in insulationDict
						where systemHostIds.Contains(kv.Key)
						select kv.Value).Cast<Element>().ToList();
					Tuple<string, string> dominantInsulationValues = GetDominantInsulationValues(doc, insulations);
					if (dominantInsulationValues != null)
					{
						thicknessInput = dominantInsulationValues.Item1;
						materialInput = dominantInsulationValues.Item2;
					}
				}
				result.Add(new SystemInsulationInfo
				{
					SystemTypeId = item2.Key,
					SystemTypeName = text,
					ElementType = (SystemElementType)1,
					ElementCount = elementCount,
					InsulatedCount = num,
					ThicknessInput = thicknessInput,
					MaterialInput = materialInput
				});
			}
			catch
			{
			}
		}
	}

	public List<SizeRangeInfo> GetSizeRanges(object document, SystemInsulationInfo system)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Expected O, but got Unknown
		List<SizeRangeInfo> list = new List<SizeRangeInfo>();
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return list;
		}
		List<Element> systemElements = GetSystemElements(val, system);
		if (!systemElements.Any())
		{
			return list;
		}
		Dictionary<ElementId, Element> insulationDict = new Dictionary<ElementId, Element>();
		foreach (PipeInsulation item4 in ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>())
		{
			try
			{
				ElementId hostElementIdForPipeInsulation = GetHostElementIdForPipeInsulation(item4);
				if (hostElementIdForPipeInsulation != (ElementId)null && hostElementIdForPipeInsulation != ElementId.InvalidElementId && !insulationDict.ContainsKey(hostElementIdForPipeInsulation))
				{
					insulationDict[hostElementIdForPipeInsulation] = (Element)(object)item4;
				}
			}
			catch
			{
			}
		}
		List<double> list2 = (from d in (from d in systemElements.Select(delegate(Element e)
				{
					try
					{
						return Math.Round(GetElementDiameter(e));
					}
					catch
					{
						return 0.0;
					}
				})
				where d > 0.0
				select d).Distinct()
			orderby d
			select d).ToList();
		if (list2.Count == 0)
		{
			return list;
		}
		List<(double, double, string)> list3 = new List<(double, double, string)>();
		double num = 0.0;
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			double num3 = list2[num2];
			(double, double, string) item2;
			if (num2 != 0)
			{
				double item = num;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
				defaultInterpolatedStringHandler.AppendFormatted(num, "0");
				defaultInterpolatedStringHandler.AppendLiteral("<d≤");
				defaultInterpolatedStringHandler.AppendFormatted(num3, "0");
				defaultInterpolatedStringHandler.AppendLiteral("mm");
				item2 = (item, num3, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("≤");
				defaultInterpolatedStringHandler2.AppendFormatted(num3, "0");
				defaultInterpolatedStringHandler2.AppendLiteral("mm");
				item2 = (0.0, num3, defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			list3.Add(item2);
			num = num3;
		}
		double item3 = num;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(3, 1);
		defaultInterpolatedStringHandler3.AppendLiteral(">");
		defaultInterpolatedStringHandler3.AppendFormatted(num, "0");
		defaultInterpolatedStringHandler3.AppendLiteral("mm");
		list3.Add((item3, double.MaxValue, defaultInterpolatedStringHandler3.ToStringAndClear()));
		foreach (var bucket in list3)
		{
			List<Element> list4 = systemElements.Where(delegate(Element e)
			{
				double elementDiameter = GetElementDiameter(e);
				return elementDiameter > bucket.Item1 && elementDiameter <= bucket.Item2;
			}).ToList();
			if (!list4.Any())
			{
				continue;
			}
			int num4 = list4.Count((Element e) => insulationDict.ContainsKey(e.Id));
			string thickness = "0";
			string material = "";
			if (num4 > 0)
			{
				List<Element> insulations = (from e in list4
					where insulationDict.ContainsKey(e.Id)
					select insulationDict[e.Id]).ToList();
				Tuple<string, string> dominantInsulationValues = GetDominantInsulationValues(val, insulations);
				if (dominantInsulationValues != null)
				{
					thickness = dominantInsulationValues.Item1;
					material = dominantInsulationValues.Item2;
				}
			}
			list.Add(new SizeRangeInfo
			{
				SizeRange = bucket.Item3,
				MinDiameter = bucket.Item1,
				MaxDiameter = bucket.Item2,
				ElementCount = list4.Count,
				InsulatedCount = num4,
				Thickness = thickness,
				Material = material
			});
		}
		return list;
	}

	public InsulationOperationSummary ExecuteSystemAdd(object document, List<SystemInsulationInfo> selectedSystems)
	{
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_0920: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		Document doc = (Document)((document is Document) ? document : null);
		if (doc == null)
		{
			return InsulationOperationSummary.Failed("document 参数无效");
		}
		Transaction val = new Transaction(doc, "RevitAi_批量添加保温");
		try
		{
			val.Start();
			try
			{
				int num = 0;
				int skipped = 0;
				List<string> list = new List<string>();
				List<SystemInsulationInfo> list2 = selectedSystems.Where(delegate(SystemInsulationInfo s)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Invalid comparison between Unknown and I4
					return (int)s.ElementType == 0;
				}).ToList();
				List<SystemInsulationInfo> list3 = selectedSystems.Where(delegate(SystemInsulationInfo s)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Invalid comparison between Unknown and I4
					return (int)s.ElementType == 1;
				}).ToList();
				if (list2.Any())
				{
					List<Pipe> source = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Pipe))).Cast<Pipe>().ToList();
					List<Element> fittings = CollectPipeFittings(doc);
					Dictionary<ElementId, PipeInsulation> insulations = (from PipeInsulation pi in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(PipeInsulation))
						select (GetHostElementIdForPipeInsulation(pi), pi: pi) into x
						where x.Item1 != ElementId.InvalidElementId
						group x by x.Item1).ToDictionary((IGrouping<ElementId, (ElementId, PipeInsulation pi)> g) => g.Key, (IGrouping<ElementId, (ElementId, PipeInsulation pi)> g) => g.First().pi);
					foreach (SystemInsulationInfo systemInfo in list2)
					{
						if (!double.TryParse(systemInfo.ThicknessInput, out var result) || result <= 0.0)
						{
							list.Add("系统 '" + systemInfo.SystemTypeName + "': 保温厚度无效");
							continue;
						}
						ElementType val2 = FindOrCreateInsulationType(doc, systemInfo.MaterialInput, result, (SystemElementType)0);
						if (val2 == null)
						{
							list.Add("系统 '" + systemInfo.SystemTypeName + "': 无法找到水管保温类型");
							continue;
						}
						List<Pipe> list4 = source.Where((Pipe p) => GetPipeSystemTypeId(doc, p) == systemInfo.SystemTypeId).ToList();
						List<Element> list5 = FilterFittingsBySystem(doc, fittings, (BuiltInParameter)(-1140334L), systemInfo.SystemTypeId);
						foreach (Pipe item in list4)
						{
							try
							{
								if (!TrySkipExisting(doc, insulations, ((Element)item).Id, systemInfo.OverrideExisting, ref skipped))
								{
									PipeInsulation.Create(doc, ((Element)item).Id, ((Element)val2).Id, result / 304.8);
									num++;
								}
							}
							catch (Exception ex)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
								defaultInterpolatedStringHandler.AppendLiteral("管道 ");
								defaultInterpolatedStringHandler.AppendFormatted(ToInt(((Element)item).Id));
								defaultInterpolatedStringHandler.AppendLiteral(": ");
								defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
								list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
							}
						}
						foreach (Element item2 in list5)
						{
							try
							{
								if (!TrySkipExisting(doc, insulations, item2.Id, systemInfo.OverrideExisting, ref skipped))
								{
									PipeInsulation.Create(doc, item2.Id, ((Element)val2).Id, result / 304.8);
									num++;
								}
							}
							catch (Exception ex2)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(5, 2);
								defaultInterpolatedStringHandler2.AppendLiteral("管件 ");
								defaultInterpolatedStringHandler2.AppendFormatted(ToInt(item2.Id));
								defaultInterpolatedStringHandler2.AppendLiteral(": ");
								defaultInterpolatedStringHandler2.AppendFormatted(ex2.Message);
								list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
							}
						}
					}
				}
				if (list3.Any())
				{
					List<Duct> source2 = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Duct))).Cast<Duct>().ToList();
					List<Element> fittings2 = CollectDuctFittings(doc);
					Dictionary<ElementId, DuctInsulation> insulations2 = (from DuctInsulation di in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation))
						select (GetHostElementIdForDuctInsulation(di), di: di) into x
						where x.Item1 != ElementId.InvalidElementId
						group x by x.Item1).ToDictionary((IGrouping<ElementId, (ElementId, DuctInsulation di)> g) => g.Key, (IGrouping<ElementId, (ElementId, DuctInsulation di)> g) => g.First().di);
					foreach (SystemInsulationInfo systemInfo2 in list3)
					{
						if (!double.TryParse(systemInfo2.ThicknessInput, out var result2) || result2 <= 0.0)
						{
							list.Add("系统 '" + systemInfo2.SystemTypeName + "': 保温厚度无效");
							continue;
						}
						ElementType val3 = FindOrCreateInsulationType(doc, systemInfo2.MaterialInput, result2, (SystemElementType)1);
						if (val3 == null)
						{
							list.Add("系统 '" + systemInfo2.SystemTypeName + "': 无法找到风管保温类型");
							continue;
						}
						List<Duct> list6 = source2.Where((Duct d) => GetDuctSystemTypeId(d) == systemInfo2.SystemTypeId).ToList();
						List<Element> list7 = FilterFittingsBySystem(doc, fittings2, (BuiltInParameter)(-1140333L), systemInfo2.SystemTypeId);
						foreach (Duct item3 in list6)
						{
							try
							{
								if (!TrySkipExisting(doc, insulations2, ((Element)item3).Id, systemInfo2.OverrideExisting, ref skipped))
								{
									DuctInsulation.Create(doc, ((Element)item3).Id, ((Element)val3).Id, result2 / 304.8);
									num++;
								}
							}
							catch (Exception ex3)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(5, 2);
								defaultInterpolatedStringHandler3.AppendLiteral("风管 ");
								defaultInterpolatedStringHandler3.AppendFormatted(ToInt(((Element)item3).Id));
								defaultInterpolatedStringHandler3.AppendLiteral(": ");
								defaultInterpolatedStringHandler3.AppendFormatted(ex3.Message);
								list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
							}
						}
						foreach (Element item4 in list7)
						{
							try
							{
								if (!TrySkipExisting(doc, insulations2, item4.Id, systemInfo2.OverrideExisting, ref skipped))
								{
									DuctInsulation.Create(doc, item4.Id, ((Element)val3).Id, result2 / 304.8);
									num++;
								}
							}
							catch (Exception ex4)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(7, 2);
								defaultInterpolatedStringHandler4.AppendLiteral("风管管件 ");
								defaultInterpolatedStringHandler4.AppendFormatted(ToInt(item4.Id));
								defaultInterpolatedStringHandler4.AppendLiteral(": ");
								defaultInterpolatedStringHandler4.AppendFormatted(ex4.Message);
								list.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
							}
						}
					}
				}
				val.Commit();
				return new InsulationOperationSummary
				{
					Added = num,
					Skipped = skipped,
					Errors = list
				};
			}
			catch (Exception ex5)
			{
				val.RollBack();
				Logger.Error("[InsulationService] 批量添加保温失败: " + ex5.Message);
				return InsulationOperationSummary.Failed("操作失败: " + ex5.Message);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private bool TrySkipExisting(Document doc, Dictionary<ElementId, PipeInsulation> insulations, ElementId hostId, bool overrideExisting, ref int skipped)
	{
		if (!insulations.TryGetValue(hostId, out PipeInsulation value))
		{
			return false;
		}
		if (overrideExisting)
		{
			doc.Delete(((Element)value).Id);
			insulations.Remove(hostId);
			return false;
		}
		skipped++;
		return true;
	}

	private bool TrySkipExisting(Document doc, Dictionary<ElementId, DuctInsulation> insulations, ElementId hostId, bool overrideExisting, ref int skipped)
	{
		if (!insulations.TryGetValue(hostId, out DuctInsulation value))
		{
			return false;
		}
		if (overrideExisting)
		{
			doc.Delete(((Element)value).Id);
			insulations.Remove(hostId);
			return false;
		}
		skipped++;
		return true;
	}

	public InsulationOperationSummary ExecuteSizeBasedAdd(object document, SystemInsulationInfo system, List<SizeRangeInfo> ranges)
	{
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected O, but got Unknown
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return InsulationOperationSummary.Failed("document 参数无效");
		}
		Transaction val2 = new Transaction(val, "RevitAi_按尺寸分档添加保温");
		try
		{
			val2.Start();
			try
			{
				int num = 0;
				List<string> list = new List<string>();
				List<Element> systemElements = GetSystemElements(val, system);
				Dictionary<ElementId, PipeInsulation> dictionary = (from PipeInsulation pi in (IEnumerable)new FilteredElementCollector(val).OfClass(typeof(PipeInsulation))
					select (GetHostElementIdForPipeInsulation(pi), pi: pi) into x
					where x.Item1 != ElementId.InvalidElementId
					group x by x.Item1).ToDictionary((IGrouping<ElementId, (ElementId, PipeInsulation pi)> g) => g.Key, (IGrouping<ElementId, (ElementId, PipeInsulation pi)> g) => g.First().pi);
				Dictionary<string, ElementType> dictionary2 = new Dictionary<string, ElementType>();
				foreach (SizeRangeInfo range in ranges)
				{
					if (!double.TryParse(range.Thickness, out var result))
					{
						continue;
					}
					string key = range.Material + "|" + range.Thickness;
					if (!dictionary2.TryGetValue(key, out var value))
					{
						value = FindOrCreateInsulationType(val, range.Material, result, (SystemElementType)0);
						if (value != null)
						{
							dictionary2[key] = value;
						}
					}
					if (value == null)
					{
						continue;
					}
					IEnumerable<Element> enumerable = systemElements.Where(delegate(Element e)
					{
						double elementDiameter = GetElementDiameter(e);
						return elementDiameter > range.MinDiameter && elementDiameter <= range.MaxDiameter;
					});
					foreach (Element item in enumerable)
					{
						try
						{
							if (dictionary.TryGetValue(item.Id, out var value2))
							{
								val.Delete(((Element)value2).Id);
								dictionary.Remove(item.Id);
							}
							PipeInsulation.Create(val, item.Id, ((Element)value).Id, result / 304.8);
							num++;
						}
						catch (Exception ex)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
							defaultInterpolatedStringHandler.AppendLiteral("元素 ");
							defaultInterpolatedStringHandler.AppendFormatted(ToInt(item.Id));
							defaultInterpolatedStringHandler.AppendLiteral(": ");
							defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
							list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
						}
					}
				}
				val2.Commit();
				return new InsulationOperationSummary
				{
					Added = num,
					Errors = list
				};
			}
			catch (Exception ex2)
			{
				val2.RollBack();
				Logger.Error("[InsulationService] 按尺寸分档添加保温失败: " + ex2.Message);
				return InsulationOperationSummary.Failed("操作失败: " + ex2.Message);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public InsulationOperationSummary ExecuteManualAdd(object document, List<SelectedElementInfo> elements, double thicknessMM, string material)
	{
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Invalid comparison between Unknown and I4
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return InsulationOperationSummary.Failed("document 参数无效");
		}
		Transaction val2 = new Transaction(val, "RevitAi_手动添加保温");
		try
		{
			val2.Start();
			try
			{
				int num = 0;
				List<string> list = new List<string>();
				foreach (SelectedElementInfo element2 in elements)
				{
					try
					{
						Element element = val.GetElement(ToId(element2.Id));
						if (element == null)
						{
							continue;
						}
						SystemElementType val3 = (element2.IsPipeRelated ? SystemElementType.Pipe : SystemElementType.Duct);
						ElementType val4 = FindOrCreateInsulationType(val, material, thicknessMM, val3);
						if (val4 == null)
						{
							continue;
						}
						if ((int)val3 == 0)
						{
							IEnumerable<PipeInsulation> enumerable = from PipeInsulation pi in (IEnumerable)new FilteredElementCollector(val).OfClass(typeof(PipeInsulation))
								where GetHostElementIdForPipeInsulation(pi) == element.Id
								select pi;
							foreach (PipeInsulation item in enumerable)
							{
								val.Delete(((Element)item).Id);
							}
							PipeInsulation.Create(val, element.Id, ((Element)val4).Id, thicknessMM / 304.8);
							num++;
							continue;
						}
						IEnumerable<DuctInsulation> enumerable2 = from DuctInsulation di in (IEnumerable)new FilteredElementCollector(val).OfClass(typeof(DuctInsulation))
							where GetHostElementIdForDuctInsulation(di) == element.Id
							select di;
						foreach (DuctInsulation item2 in enumerable2)
						{
							val.Delete(((Element)item2).Id);
						}
						DuctInsulation.Create(val, element.Id, ((Element)val4).Id, thicknessMM / 304.8);
						num++;
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler.AppendFormatted(element2.Id);
						defaultInterpolatedStringHandler.AppendLiteral(": ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val2.Commit();
				return new InsulationOperationSummary
				{
					Added = num,
					Errors = list
				};
			}
			catch (Exception ex2)
			{
				val2.RollBack();
				Logger.Error("[InsulationService] 手动添加保温失败: " + ex2.Message);
				return InsulationOperationSummary.Failed("操作失败: " + ex2.Message);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public string? IsolateElements(List<int> elementIds)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		UIDocument activeUIDocument = ActiveUIDocument;
		if (activeUIDocument == null)
		{
			return "当前没有活动文档";
		}
		View activeView = activeUIDocument.ActiveView;
		if (activeView == null)
		{
			return "当前没有活动视图";
		}
		try
		{
			List<ElementId> list = elementIds.Select(ToId).ToList();
			list.AddRange(GetInsulationIdsForHosts(activeUIDocument.Document, list));
			Transaction val = new Transaction(activeUIDocument.Document, "RevitAi_隔离元素");
			try
			{
				val.Start();
				if (activeView.IsTemporaryHideIsolateActive())
				{
					activeView.DisableTemporaryViewMode((TemporaryViewMode)2);
				}
				activeView.IsolateElementsTemporary((ICollection<ElementId>)list);
				val.Commit();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			activeUIDocument.Selection.SetElementIds((ICollection<ElementId>)list);
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string? IsolateSystem(object document, SystemInsulationInfo system, out int elementCount)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		elementCount = 0;
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return "document 参数无效";
		}
		UIDocument activeUIDocument = ActiveUIDocument;
		if (activeUIDocument == null)
		{
			return "当前没有活动文档";
		}
		View activeView = activeUIDocument.ActiveView;
		if (activeView == null)
		{
			return "当前没有活动视图";
		}
		try
		{
			List<Element> systemElements = GetSystemElements(val, system);
			if (!systemElements.Any())
			{
				return "系统中没有可隔离的元素";
			}
			List<ElementId> list = systemElements.Select((Element e) => e.Id).ToList();
			list.AddRange(GetInsulationIdsForHosts(val, list));
			Transaction val2 = new Transaction(val, "RevitAi_隔离系统");
			try
			{
				val2.Start();
				if (activeView.IsTemporaryHideIsolateActive())
				{
					activeView.DisableTemporaryViewMode((TemporaryViewMode)2);
				}
				activeView.IsolateElementsTemporary((ICollection<ElementId>)list);
				val2.Commit();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			activeUIDocument.Selection.SetElementIds((ICollection<ElementId>)list);
			elementCount = list.Count;
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string RestoreDisplay()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		UIDocument activeUIDocument = ActiveUIDocument;
		if (activeUIDocument == null)
		{
			return "当前没有活动文档";
		}
		View activeView = activeUIDocument.ActiveView;
		if (activeView == null)
		{
			return "当前没有活动视图";
		}
		if (activeView.IsTemporaryHideIsolateActive())
		{
			Transaction val = new Transaction(activeUIDocument.Document, "RevitAi_恢复显示");
			try
			{
				val.Start();
				activeView.DisableTemporaryViewMode((TemporaryViewMode)2);
				val.Commit();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return "已取消视图「" + ((Element)activeView).Name + "」的临时隐藏/隔离";
		}
		return "当前视图中没有临时隐藏/隔离状态";
	}

	public void SelectElementIds(List<int> elementIds)
	{
		UIDocument activeUIDocument = ActiveUIDocument;
		if (activeUIDocument != null)
		{
			activeUIDocument.Selection.SetElementIds((ICollection<ElementId>)elementIds.Select(ToId).ToList());
		}
	}

	public int SelectUninsulated(object document, SystemInsulationInfo system)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I4
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return 0;
		}
		List<Element> systemElements = GetSystemElements(val, system);
		if (!systemElements.Any())
		{
			return 0;
		}
		HashSet<ElementId> insulationHostIds = new HashSet<ElementId>();
		if ((int)system.ElementType == 0)
		{
			foreach (PipeInsulation item in ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(PipeInsulation))).Cast<PipeInsulation>())
			{
				try
				{
					ElementId hostElementIdForPipeInsulation = GetHostElementIdForPipeInsulation(item);
					if (hostElementIdForPipeInsulation != (ElementId)null && hostElementIdForPipeInsulation != ElementId.InvalidElementId)
					{
						insulationHostIds.Add(hostElementIdForPipeInsulation);
					}
				}
				catch
				{
				}
			}
		}
		else
		{
			foreach (DuctInsulation item2 in ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(DuctInsulation))).Cast<DuctInsulation>())
			{
				try
				{
					ElementId hostElementIdForDuctInsulation = GetHostElementIdForDuctInsulation(item2);
					if (hostElementIdForDuctInsulation != (ElementId)null && hostElementIdForDuctInsulation != ElementId.InvalidElementId)
					{
						insulationHostIds.Add(hostElementIdForDuctInsulation);
					}
				}
				catch
				{
				}
			}
		}
		List<ElementId> list = (from e in systemElements
			where !insulationHostIds.Contains(e.Id)
			select e.Id).ToList();
		if (!list.Any())
		{
			return 0;
		}
		UIDocument activeUIDocument = ActiveUIDocument;
		if (activeUIDocument != null)
		{
			activeUIDocument.Selection.SetElementIds((ICollection<ElementId>)list);
		}
		return list.Count;
	}

	public List<SelectedElementInfo> PickInsulationElements(string prompt)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		List<SelectedElementInfo> list = new List<SelectedElementInfo>();
		UIDocument activeUIDocument = ActiveUIDocument;
		if (activeUIDocument == null)
		{
			return list;
		}
		Document document = activeUIDocument.Document;
		try
		{
			IList<Reference> list2 = activeUIDocument.Selection.PickObjects((ObjectType)1, (ISelectionFilter)(object)new InsulationSelectionFilter(), prompt);
			foreach (Reference item in list2)
			{
				Element element = document.GetElement(item);
				if (element == null)
				{
					continue;
				}
				bool flag = element is Pipe;
				bool flag2 = element is Duct;
				if (!flag && !flag2 && element.Category == null)
				{
					continue;
				}
				bool isPipeRelated;
				if (flag)
				{
					isPipeRelated = true;
				}
				else if (flag2)
				{
					isPipeRelated = false;
				}
				else
				{
					int num = ToInt(element.Category.Id);
					if (num == -2008049 || num == -2008055)
					{
						isPipeRelated = true;
					}
					else
					{
						if (num != -2008010 && num != -2008016)
						{
							continue;
						}
						isPipeRelated = false;
					}
				}
				double elementDiameter = GetElementDiameter(element);
				SelectedElementInfo val = new SelectedElementInfo
				{
					Id = ToInt(element.Id),
					ElementType = GetElementTypeName(element),
					SystemName = GetElementSystemName(document, element)
				};
				string size;
				if (!(elementDiameter > 0.0))
				{
					size = "-";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
					defaultInterpolatedStringHandler.AppendFormatted(elementDiameter, "F0");
					defaultInterpolatedStringHandler.AppendLiteral("mm");
					size = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				val.Size = size;
				val.IsPipeRelated = isPipeRelated;
				list.Add(val);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			Logger.Error("[InsulationService] 拾取元素失败: " + ex2.Message);
		}
		return list;
	}

	private static List<Element> CollectPipeFittings(Document doc)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable<Element>)new FilteredElementCollector(doc).WhereElementIsNotElementType()).Where(delegate(Element e)
		{
			if (e.Category == null)
			{
				return false;
			}
			int num = ToInt(e.Category.Id);
			return num == -2008049 || num == -2008055 || num == -2000300;
		}).ToList();
	}

	private static List<Element> CollectDuctFittings(Document doc)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable<Element>)new FilteredElementCollector(doc).WhereElementIsNotElementType()).Where(delegate(Element e)
		{
			if (e.Category == null)
			{
				return false;
			}
			int num = ToInt(e.Category.Id);
			return num == -2008010 || num == -2008016 || num == -2000350;
		}).ToList();
	}

	private static List<Element> FilterFittingsBySystem(Document doc, List<Element> fittings, BuiltInParameter paramType, int systemTypeId)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		List<Element> list = new List<Element>();
		foreach (Element fitting in fittings)
		{
			try
			{
				Parameter val = fitting.get_Parameter(paramType);
				if (val != null && val.HasValue && (int)val.StorageType == 4 && ToInt(val.AsElementId()) == systemTypeId)
				{
					list.Add(fitting);
				}
			}
			catch
			{
			}
		}
		return list;
	}

	private static List<ElementId> GetInsulationIdsForHosts(Document doc, ICollection<ElementId> hostIds)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		List<ElementId> list = new List<ElementId>();
		if (hostIds == null || !hostIds.Any())
		{
			return list;
		}
		HashSet<ElementId> hostIdSet = new HashSet<ElementId>(hostIds);
		list.AddRange(from PipeInsulation pi in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(PipeInsulation))
			where hostIdSet.Contains(GetHostElementIdForPipeInsulation(pi))
			select ((Element)pi).Id);
		list.AddRange(from DuctInsulation di in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation))
			where hostIdSet.Contains(GetHostElementIdForDuctInsulation(di))
			select ((Element)di).Id);
		return list;
	}

	private static int GetPipeSystemTypeId(Document doc, Pipe pipe)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Invalid comparison between Unknown and I4
		try
		{
			PropertyInfo property = ((object)pipe).GetType().GetProperty("SystemTypeId");
			if (property != null)
			{
				object value = property.GetValue(pipe);
				ElementId val = (ElementId)((value is ElementId) ? value : null);
				if (val != null)
				{
					return ToInt(val);
				}
			}
			foreach (Parameter parameter in ((Element)pipe).Parameters)
			{
				Parameter val2 = parameter;
				if ((val2.Definition.Name.Contains("System") || val2.Definition.Name.Contains("系统")) && (int)val2.StorageType == 4)
				{
					ElementId val3 = val2.AsElementId();
					if (doc.GetElement(val3) is PipingSystemType)
					{
						return ToInt(val3);
					}
				}
			}
			return -1;
		}
		catch
		{
			return -1;
		}
	}

	private static int GetDuctSystemTypeId(Duct duct)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)duct).get_Parameter((BuiltInParameter)(-1140333L));
			if (val != null && val.HasValue && (int)val.StorageType == 4)
			{
				return ToInt(val.AsElementId());
			}
			return -1;
		}
		catch
		{
			return -1;
		}
	}

	private static ElementId GetHostElementIdForPipeInsulation(PipeInsulation insulation)
	{
		try
		{
			Type type = ((object)insulation).GetType();
			if ((object)_pipeHostElementIdProperty == null)
			{
				_pipeHostElementIdProperty = type.GetProperty("HostElementId");
			}
			if (_pipeHostElementIdProperty != null)
			{
				object value = _pipeHostElementIdProperty.GetValue(insulation);
				ElementId val = (ElementId)((value is ElementId) ? value : null);
				if (val != null && val != ElementId.InvalidElementId)
				{
					return val;
				}
			}
			if ((object)_pipeIdProperty == null)
			{
				_pipeIdProperty = type.GetProperty("PipeId");
			}
			if (_pipeIdProperty != null)
			{
				object? value2 = _pipeIdProperty.GetValue(insulation);
				ElementId val2 = (ElementId)((value2 is ElementId) ? value2 : null);
				if (val2 != null && val2 != ElementId.InvalidElementId)
				{
					return val2;
				}
			}
			return GetHostIdFromParameters((Element)(object)insulation) ?? ElementId.InvalidElementId;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	private static ElementId GetHostElementIdForDuctInsulation(DuctInsulation insulation)
	{
		try
		{
			Type type = ((object)insulation).GetType();
			if ((object)_ductHostElementIdProperty == null)
			{
				_ductHostElementIdProperty = type.GetProperty("HostElementId");
			}
			if (_ductHostElementIdProperty != null)
			{
				object value = _ductHostElementIdProperty.GetValue(insulation);
				ElementId val = (ElementId)((value is ElementId) ? value : null);
				if (val != null && val != ElementId.InvalidElementId)
				{
					return val;
				}
			}
			if ((object)_ductIdProperty == null)
			{
				_ductIdProperty = type.GetProperty("DuctId");
			}
			if (_ductIdProperty != null)
			{
				object? value2 = _ductIdProperty.GetValue(insulation);
				ElementId val2 = (ElementId)((value2 is ElementId) ? value2 : null);
				if (val2 != null && val2 != ElementId.InvalidElementId)
				{
					return val2;
				}
			}
			return GetHostIdFromParameters((Element)(object)insulation) ?? ElementId.InvalidElementId;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	private static ElementId? GetHostIdFromParameters(Element insulation)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		foreach (Parameter parameter in insulation.Parameters)
		{
			Parameter val = parameter;
			string name = val.Definition.Name;
			if ((name.Contains("Host") || name.Contains("host") || name.Contains("主机") || name.Contains("关联") || name.Contains("Reference") || name.Contains("Owner")) && (int)val.StorageType == 4 && val.HasValue)
			{
				return val.AsElementId();
			}
		}
		return null;
	}

	private static double GetElementDiameter(Element element)
	{
		try
		{
			Pipe val = (Pipe)(object)((element is Pipe) ? element : null);
			if (val != null)
			{
				return ((MEPCurve)val).Diameter * 304.8;
			}
			Duct val2 = (Duct)(object)((element is Duct) ? element : null);
			if (val2 != null)
			{
				if (((MEPCurve)val2).Diameter > 0.0)
				{
					return ((MEPCurve)val2).Diameter * 304.8;
				}
				double width = ((MEPCurve)val2).Width;
				double height = ((MEPCurve)val2).Height;
				if (width > 0.0 && height > 0.0)
				{
					return 2.0 * (width + height) / Math.PI * 304.8;
				}
			}
			else if (element.Category != null)
			{
				double fittingDiameterMm = GetFittingDiameterMm(element);
				if (fittingDiameterMm > 0.0)
				{
					return fittingDiameterMm;
				}
			}
		}
		catch
		{
		}
		return 0.0;
	}

	private static double GetFittingDiameterMm(Element element)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		try
		{
			FamilyInstance val = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			if (val != null && val.MEPModel != null)
			{
				ConnectorManager connectorManager = val.MEPModel.ConnectorManager;
				if (connectorManager != null)
				{
					double num = 0.0;
					foreach (Connector connector in connectorManager.Connectors)
					{
						Connector val2 = connector;
						try
						{
							if ((int)val2.Shape == 0 && val2.Radius > 0.0)
							{
								num = Math.Max(num, val2.Radius * 2.0);
							}
							else if ((int)val2.Shape == 1 && val2.Width > 0.0 && val2.Height > 0.0)
							{
								double val3 = 2.0 * (val2.Width + val2.Height) / Math.PI;
								num = Math.Max(num, val3);
							}
						}
						catch
						{
						}
					}
					if (num > 0.0)
					{
						return num * 304.8;
					}
				}
			}
			Parameter val4 = element.get_Parameter((BuiltInParameter)(-1140225L));
			if (val4 != null && val4.HasValue)
			{
				return val4.AsDouble() * 304.8;
			}
		}
		catch
		{
		}
		return 0.0;
	}

	private static List<Element> GetSystemElements(Document doc, SystemInsulationInfo systemInfo)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<Element> list = new List<Element>();
		if ((int)systemInfo.ElementType == 0)
		{
			list.AddRange((IEnumerable<Element>)(from Pipe p in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Pipe))
				where GetPipeSystemTypeId(doc, p) == systemInfo.SystemTypeId
				select p));
			list.AddRange(FilterFittingsBySystem(doc, CollectPipeFittings(doc), (BuiltInParameter)(-1140334L), systemInfo.SystemTypeId));
		}
		else
		{
			list.AddRange((IEnumerable<Element>)(from Duct d in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Duct))
				where GetDuctSystemTypeId(d) == systemInfo.SystemTypeId
				select d));
			list.AddRange(FilterFittingsBySystem(doc, CollectDuctFittings(doc), (BuiltInParameter)(-1140333L), systemInfo.SystemTypeId));
		}
		return list;
	}

	private static ElementId GetInsulationTypeId(Document doc, Element insulation)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Invalid comparison between Unknown and I4
		try
		{
			foreach (Parameter parameter in insulation.Parameters)
			{
				Parameter val = parameter;
				if ((int)val.StorageType != 4 || !val.HasValue)
				{
					continue;
				}
				Definition definition = val.Definition;
				object obj;
				if (definition == null)
				{
					obj = null;
				}
				else
				{
					obj = definition.Name;
					if (obj != null)
					{
						goto IL_0059;
					}
				}
				obj = "";
				goto IL_0059;
				IL_0059:
				string text = (string)obj;
				if (!text.Contains("Type") && !text.Contains("类型"))
				{
					continue;
				}
				ElementId val2 = val.AsElementId();
				if (!(val2 == (ElementId)null) && !(val2 == ElementId.InvalidElementId))
				{
					Element element = doc.GetElement(val2);
					if (element is PipeInsulationType || element is DuctInsulationType)
					{
						return val2;
					}
				}
			}
			foreach (Parameter parameter2 in insulation.Parameters)
			{
				Parameter val3 = parameter2;
				if ((int)val3.StorageType != 4 || !val3.HasValue)
				{
					continue;
				}
				ElementId val4 = val3.AsElementId();
				if (!(val4 == (ElementId)null) && !(val4 == ElementId.InvalidElementId))
				{
					Element element2 = doc.GetElement(val4);
					if (element2 is PipeInsulationType || element2 is DuctInsulationType)
					{
						return val4;
					}
				}
			}
		}
		catch
		{
		}
		return ElementId.InvalidElementId;
	}

	private static Tuple<string, string>? GetDominantInsulationValues(Document doc, IEnumerable<Element> insulations)
	{
		List<Element> list = insulations?.Where((Element i) => i != null).ToList();
		if (list == null || list.Count == 0)
		{
			return null;
		}
		Dictionary<ElementId, string> materialCache = new Dictionary<ElementId, string>();
		var source = list.Select(delegate(Element ins)
		{
			ElementId insulationTypeId = GetInsulationTypeId(doc, ins);
			if (!materialCache.TryGetValue(insulationTypeId, out string value) || value == null)
			{
				value = GetInsulationMaterialFromType(doc, insulationTypeId);
				materialCache[insulationTypeId] = value;
			}
			return new
			{
				Thickness = GetInsulationThickness(doc, ins),
				Material = (value ?? "")
			};
		}).ToList();
		var grouping = (from p in source
			group p by p into g
			orderby g.Count() descending
			select g).First();
		return Tuple.Create(grouping.Key.Thickness.ToString("F0"), grouping.Key.Material);
	}

	private static double GetInsulationThickness(Document doc, Element insulation)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((IEnumerable)insulation.Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Contains("隔热层厚度") || p.Definition.Name.Contains("Insulation Thickness") || p.Definition.Name.Contains("Thickness") || p.Definition.Name.Contains("厚度"));
			if (val != null && val.HasValue && (int)val.StorageType == 2)
			{
				double num = val.AsDouble();
				if (num > 0.0)
				{
					return num * 304.8;
				}
			}
			ElementId insulationTypeId = GetInsulationTypeId(doc, insulation);
			if (insulationTypeId != ElementId.InvalidElementId)
			{
				Element element = doc.GetElement(insulationTypeId);
				if (element != null)
				{
					Parameter val2 = ((IEnumerable)element.Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Contains("Thickness") || p.Definition.Name.Contains("厚度"));
					if (val2 != null && val2.HasValue && (int)val2.StorageType == 2)
					{
						return val2.AsDouble() * 304.8;
					}
				}
			}
			return 0.0;
		}
		catch
		{
			return 0.0;
		}
	}

	private static string GetInsulationMaterialFromType(Document doc, ElementId typeId)
	{
		try
		{
			if (typeId != ElementId.InvalidElementId)
			{
				Element element = doc.GetElement(typeId);
				if (element != null)
				{
					Parameter val = element.get_Parameter((BuiltInParameter)(-1002001L));
					if (val != null && val.HasValue)
					{
						return val.AsString() ?? "";
					}
					return element.Name ?? "";
				}
			}
			return "";
		}
		catch
		{
			return "";
		}
	}

	private static ElementType? FindOrCreateInsulationType(Document doc, string materialName, double thicknessMM, SystemElementType elementType)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		List<ElementType> source = ((IEnumerable)new FilteredElementCollector(doc).OfClass(((int)elementType == 0) ? typeof(PipeInsulationType) : typeof(DuctInsulationType))).Cast<ElementType>().ToList();
		ElementType val = source.FirstOrDefault(delegate(ElementType t)
		{
			Parameter val5 = ((Element)t).get_Parameter((BuiltInParameter)(-1002001L));
			if (val5 == null)
			{
				return false;
			}
			string text = val5.AsString() ?? ((Element)t).Name;
			return text.Equals(materialName, StringComparison.OrdinalIgnoreCase) || text.Contains(materialName);
		});
		if (val != null)
		{
			return val;
		}
		try
		{
			ElementType val2 = source.FirstOrDefault();
			if (val2 == null)
			{
				return null;
			}
			ElementType val3 = val2.Duplicate(materialName);
			if (val3 == null)
			{
				return null;
			}
			foreach (Parameter parameter in ((Element)val3).Parameters)
			{
				Parameter val4 = parameter;
				if (val4.Definition.Name.Equals("厚度", StringComparison.OrdinalIgnoreCase) || val4.Definition.Name.Equals("Thickness", StringComparison.OrdinalIgnoreCase))
				{
					val4.Set(thicknessMM / 304.8);
				}
			}
			return val3;
		}
		catch (Exception ex)
		{
			Logger.Error("[InsulationService] 创建保温类型 '" + materialName + "' 失败: " + ex.Message);
			return null;
		}
	}

	private static string GetElementSystemName(Document doc, Element element)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Invalid comparison between Unknown and I4
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Invalid comparison between Unknown and I4
		try
		{
			Pipe val = (Pipe)(object)((element is Pipe) ? element : null);
			object obj2;
			object obj3;
			object obj5;
			object obj6;
			if (val != null)
			{
				int pipeSystemTypeId = GetPipeSystemTypeId(doc, val);
				if (pipeSystemTypeId >= 0)
				{
					Element element2 = doc.GetElement(ToId(pipeSystemTypeId));
					Element obj = ((element2 is PipingSystemType) ? element2 : null);
					if (obj == null)
					{
						obj2 = null;
					}
					else
					{
						obj2 = obj.Name;
						if (obj2 != null)
						{
							goto IL_004d;
						}
					}
					obj2 = "未知系统";
					goto IL_004d;
				}
			}
			else
			{
				Duct val2 = (Duct)(object)((element is Duct) ? element : null);
				if (val2 != null)
				{
					int ductSystemTypeId = GetDuctSystemTypeId(val2);
					if (ductSystemTypeId >= 0)
					{
						Element element3 = doc.GetElement(ToId(ductSystemTypeId));
						if (element3 == null)
						{
							obj3 = null;
						}
						else
						{
							obj3 = element3.Name;
							if (obj3 != null)
							{
								goto IL_009c;
							}
						}
						obj3 = "未知系统";
						goto IL_009c;
					}
				}
				else if (element.Category != null)
				{
					int num = ToInt(element.Category.Id);
					if (num == -2008049 || num == -2008055)
					{
						Parameter val3 = element.get_Parameter((BuiltInParameter)(-1140334L));
						if (val3 != null && val3.HasValue && (int)val3.StorageType == 4)
						{
							Element element4 = doc.GetElement(val3.AsElementId());
							Element obj4 = ((element4 is PipingSystemType) ? element4 : null);
							if (obj4 == null)
							{
								obj5 = null;
							}
							else
							{
								obj5 = obj4.Name;
								if (obj5 != null)
								{
									goto IL_0135;
								}
							}
							obj5 = "未知系统";
							goto IL_0135;
						}
					}
					else if (num == -2008010 || num == -2008016)
					{
						Parameter val4 = element.get_Parameter((BuiltInParameter)(-1140333L));
						if (val4 != null && val4.HasValue && (int)val4.StorageType == 4)
						{
							Element element5 = doc.GetElement(val4.AsElementId());
							if (element5 == null)
							{
								obj6 = null;
							}
							else
							{
								obj6 = element5.Name;
								if (obj6 != null)
								{
									goto IL_01a3;
								}
							}
							obj6 = "未知系统";
							goto IL_01a3;
						}
					}
				}
			}
			goto end_IL_0001;
			IL_0135:
			return (string)obj5;
			IL_01a3:
			return (string)obj6;
			IL_009c:
			return (string)obj3;
			IL_004d:
			return (string)obj2;
			end_IL_0001:;
		}
		catch
		{
		}
		return "未知系统";
	}

	private static string GetElementTypeName(Element element)
	{
		if (element is Pipe)
		{
			return "管道";
		}
		if (element is Duct)
		{
			return "风管";
		}
		if (element.Category == null)
		{
			return "其他";
		}
		return ToInt(element.Category.Id) switch
		{
			-2008049 => "管件", 
			-2008055 => "配件", 
			-2008010 => "风管管件", 
			-2008016 => "风管配件", 
			_ => "其他", 
		};
	}
}
