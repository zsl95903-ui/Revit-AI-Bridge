using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using ns6;

namespace RevitAi.Revit.Services;

public sealed class PipeCreationService : IPipeCreationService
{
	public object? CreatePipe(object document, EnrichedPipeData pipeData, PipeNetworkModelingOptions options)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			PipeType orCreateUnifiedPipeType = GetOrCreateUnifiedPipeType(val, options.PipeSystemName);
			if (orCreateUnifiedPipeType == null)
			{
				return null;
			}
			XYZ val2 = new XYZ(pipeData.StartX / 304.8, pipeData.StartY / 304.8, pipeData.StartZ / 304.8);
			XYZ val3 = new XYZ(pipeData.EndX / 304.8, pipeData.EndY / 304.8, pipeData.EndZ / 304.8);
			PipingSystemType pipingSystemType = GetPipingSystemType(val);
			if (pipingSystemType == null)
			{
				return null;
			}
			Level orCreateLevel = GetOrCreateLevel(val, val2.Z);
			if (orCreateLevel == null)
			{
				return null;
			}
			Pipe val4 = Pipe.Create(val, ((Element)pipingSystemType).Id, GetElementId(orCreateUnifiedPipeType), ((Element)orCreateLevel).Id, val2, val3);
			if (val4 == null)
			{
				return null;
			}
			SetPipeDiameter(val4, pipeData.DiameterMM);
			return val4;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private PipeType? GetOrCreateUnifiedPipeType(Document doc, string systemName)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			PipeType val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(PipeType))).Cast<PipeType>().FirstOrDefault((PipeType pt) => ((Element)pt).Name.Equals(systemName, StringComparison.OrdinalIgnoreCase));
			if (val != null)
			{
				return val;
			}
			return CreateNewUnifiedPipeType(doc, systemName);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public PipeCreationResult CreatePipes(object document, List<EnrichedPipeData> pipes, PipeNetworkModelingOptions options, Action<int, int>? progressCallback = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		PipeCreationResult val = new PipeCreationResult();
		Document val2 = (Document)((document is Document) ? document : null);
		if (val2 == null)
		{
			val.Errors.Add("文档类型无效");
			return val;
		}
		for (int i = 0; i < pipes.Count; i++)
		{
			try
			{
				progressCallback?.Invoke(i + 1, pipes.Count);
				object obj = CreatePipe(val2, pipes[i], options);
				if (obj != null)
				{
					int successCount = val.SuccessCount;
					val.SuccessCount = successCount + 1;
					Pipe val3 = (Pipe)((obj is Pipe) ? obj : null);
					if (val3 != null)
					{
						val.CreatedPipeIds.Add((int)((Element)val3).Id.Value);
					}
				}
				else
				{
					int successCount = val.FailureCount;
					val.FailureCount = successCount + 1;
					List<string> errors = val.Errors;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
					defaultInterpolatedStringHandler.AppendLiteral("管道 ");
					defaultInterpolatedStringHandler.AppendFormatted(i + 1);
					defaultInterpolatedStringHandler.AppendLiteral(" 创建失败");
					errors.Add(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			catch (Exception ex)
			{
				int successCount = val.FailureCount;
				val.FailureCount = successCount + 1;
				List<string> errors2 = val.Errors;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(5, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("管道 ");
				defaultInterpolatedStringHandler2.AppendFormatted(i + 1);
				defaultInterpolatedStringHandler2.AppendLiteral(": ");
				defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
				errors2.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
		}
		return val;
	}

	private PipeType? CreateNewUnifiedPipeType(Document doc, string systemName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			PipeType val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(PipeType))).Cast<PipeType>().FirstOrDefault();
			if (val == null)
			{
				return null;
			}
			ElementType obj = ((ElementType)val).Duplicate(systemName);
			PipeType val2 = (PipeType)(object)((obj is PipeType) ? obj : null);
			if (val2 == null)
			{
				return null;
			}
			return val2;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private void SetPipeDiameter(Pipe pipe, double diameterMM)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)pipe).get_Parameter((BuiltInParameter)(-1140225L));
			if (val != null && (int)val.StorageType == 2)
			{
				val.Set(diameterMM / 304.8);
			}
		}
		catch (Exception)
		{
		}
	}

	private ElementId GetElementId(object element)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		Element val = (Element)((element is Element) ? element : null);
		if (val == null)
		{
			throw new ArgumentException("无法获取元素 ID");
		}
		return new ElementId(val.Id.Value);
	}

	private Level? GetOrCreateLevel(Document doc, double elevationFeet)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Level val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Level))).Cast<Level>().FirstOrDefault((Level l) => Math.Abs(l.Elevation - elevationFeet) < 1.0);
			if (val != null)
			{
				return val;
			}
			return (from Level l in (IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Level))
				orderby Math.Abs(l.Elevation - elevationFeet)
				select l).FirstOrDefault();
		}
		catch (Exception)
		{
			return null;
		}
	}

	private PipingSystemType? GetPipingSystemType(Document doc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType))).Cast<PipingSystemType>().FirstOrDefault();
		}
		catch (Exception)
		{
			return null;
		}
	}
}
