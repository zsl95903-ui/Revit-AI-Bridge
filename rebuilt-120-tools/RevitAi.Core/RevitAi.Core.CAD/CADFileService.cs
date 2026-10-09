using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using CSMath;
using ns7;

namespace RevitAi.Core.CAD;

internal sealed class CADFileService : ICADFileService
{
	[CompilerGenerated]
	public sealed class Class86
	{
		public string string_0;

		internal bool method_0(CADLayerInfo cadlayerInfo_0)
		{
			return cadlayerInfo_0.Name == string_0;
		}

		internal bool method_1(CADTextInfo cadtextInfo_0)
		{
			return cadtextInfo_0.LayerName == string_0;
		}

		internal bool method_2(CADBlockInfo cadblockInfo_0)
		{
			return cadblockInfo_0.LayerName == string_0;
		}

		internal bool method_3(CADLineInfo cadlineInfo_0)
		{
			return cadlineInfo_0.LayerName == string_0;
		}
	}

	CADFileInfo? ICADFileService.GetCADFilePath(object importInstance, object document)
	{
		Logger.Warning("[CADFileService] Core 层无法直接获取 CAD 文件路径，请使用适配层的实现");
		return null;
	}

	CADFileData? ICADFileService.ParseCADFile(string filePath, double conversionFactorToMM)
	{
		//IL_0e5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f87: Expected O, but got Unknown
		//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0924: Unknown result type (might be due to invalid IL or missing references)
		//IL_0927: Unknown result type (might be due to invalid IL or missing references)
		//IL_092c: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_093f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Expected O, but got Unknown
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Expected O, but got Unknown
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Expected O, but got Unknown
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected O, but got Unknown
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_013d: Expected O, but got Unknown
		//IL_129a: Unknown result type (might be due to invalid IL or missing references)
		//IL_129f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1368: Unknown result type (might be due to invalid IL or missing references)
		//IL_136d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1377: Unknown result type (might be due to invalid IL or missing references)
		//IL_1381: Unknown result type (might be due to invalid IL or missing references)
		//IL_138b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1395: Unknown result type (might be due to invalid IL or missing references)
		//IL_139f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_13dd: Expected O, but got Unknown
		//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_101b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1020: Unknown result type (might be due to invalid IL or missing references)
		//IL_102b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1030: Unknown result type (might be due to invalid IL or missing references)
		//IL_106e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1073: Unknown result type (might be due to invalid IL or missing references)
		//IL_107d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Unknown result type (might be due to invalid IL or missing references)
		//IL_1091: Unknown result type (might be due to invalid IL or missing references)
		//IL_109b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10af: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e3: Expected O, but got Unknown
		//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9e: Expected O, but got Unknown
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Expected O, but got Unknown
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_140c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1411: Unknown result type (might be due to invalid IL or missing references)
		//IL_141c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1421: Unknown result type (might be due to invalid IL or missing references)
		//IL_1456: Unknown result type (might be due to invalid IL or missing references)
		//IL_145b: Unknown result type (might be due to invalid IL or missing references)
		//IL_146e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1473: Unknown result type (might be due to invalid IL or missing references)
		//IL_147e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1483: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14db: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_150b: Unknown result type (might be due to invalid IL or missing references)
		//IL_151b: Unknown result type (might be due to invalid IL or missing references)
		//IL_152d: Expected O, but got Unknown
		//IL_113f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1144: Unknown result type (might be due to invalid IL or missing references)
		//IL_114f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1154: Unknown result type (might be due to invalid IL or missing references)
		//IL_115f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1164: Unknown result type (might be due to invalid IL or missing references)
		//IL_1199: Unknown result type (might be due to invalid IL or missing references)
		//IL_119e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11be: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1202: Unknown result type (might be due to invalid IL or missing references)
		//IL_120c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1216: Unknown result type (might be due to invalid IL or missing references)
		//IL_1220: Unknown result type (might be due to invalid IL or missing references)
		//IL_122a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1234: Unknown result type (might be due to invalid IL or missing references)
		//IL_1246: Unknown result type (might be due to invalid IL or missing references)
		//IL_1256: Unknown result type (might be due to invalid IL or missing references)
		//IL_1268: Expected O, but got Unknown
		//IL_0cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d81: Expected O, but got Unknown
		//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7e: Expected O, but got Unknown
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Expected O, but got Unknown
		try
		{
			if (!File.Exists(filePath))
			{
				Logger.Error("[CADFileService] 文件不存在: " + filePath);
				return null;
			}
			string text = Path.GetExtension(filePath).ToLower();
			if (text != ".dwg" && text != ".dxf")
			{
				Logger.Error("[CADFileService] 不支持的文件格式: " + text);
				return null;
			}
			Logger.Info("[CADFileService] 解析 CAD 文件（显式单位）: " + filePath);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CADFileService] 单位转换系数: ");
			defaultInterpolatedStringHandler.AppendFormatted(conversionFactorToMM);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			CadDocument val = null;
			try
			{
				val = ((!(text == ".dwg")) ? DxfReader.Read(filePath, (NotificationEventHandler)null) : DwgReader.Read(filePath, (NotificationEventHandler)null));
			}
			catch (Exception ex)
			{
				Logger.Error("[CADFileService] ACadSharp 读取文件失败: " + ex.Message);
				return null;
			}
			if (val == null)
			{
				Logger.Error("[CADFileService] 无法加载 CAD 文件: " + filePath);
				return null;
			}
			CADFileData val2 = new CADFileData
			{
				FileInfo = new CADFileInfo
				{
					FilePath = filePath
				}
			};
			val2.Layers = ((IEnumerable<Layer>)val.Layers).Select((Func<Layer, CADLayerInfo>)delegate(Layer layer_0)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Expected O, but got Unknown
				CADLayerInfo val36 = new CADLayerInfo
				{
					Name = ((TableEntry)layer_0).Name
				};
				Color color = layer_0.Color;
				val36.ColorIndex = ((Color)color).Index;
				val36.IsVisible = layer_0.IsOn;
				val36.IsLocked = false;
				LineType lineType = layer_0.LineType;
				val36.LineTypeName = ((lineType != null) ? ((TableEntry)lineType).Name : null);
				val36.LineWeight = null;
				return val36;
			}).ToList();
			List<CADTextInfo> list = new List<CADTextInfo>();
			XYZ val5;
			foreach (Entity entity in val.Entities)
			{
				try
				{
					TextEntity val3 = (TextEntity)(object)((entity is TextEntity) ? entity : null);
					if (val3 != null)
					{
						double num = (double)val3.Value.Length * val3.Height * val3.WidthFactor;
						CADTextInfo val4 = new CADTextInfo
						{
							Content = val3.Value
						};
						val5 = val3.InsertPoint;
						val4.X = ((XYZ)val5).X * conversionFactorToMM;
						val5 = val3.InsertPoint;
						val4.Y = ((XYZ)val5).Y * conversionFactorToMM;
						val5 = val3.InsertPoint;
						val4.Z = ((XYZ)val5).Z * conversionFactorToMM;
						val4.LayerName = ((TableEntry)((Entity)val3).Layer).Name;
						val4.Height = val3.Height * conversionFactorToMM;
						val4.Width = num * conversionFactorToMM;
						val4.Rotation = val3.Rotation;
						val4.HorizontalAlignment = ((object)val3.HorizontalAlignment/*cast due to constrained. prefix*/).ToString();
						val4.VerticalAlignment = ((object)val3.VerticalAlignment/*cast due to constrained. prefix*/).ToString();
						list.Add(val4);
						continue;
					}
					MText val6 = (MText)(object)((entity is MText) ? entity : null);
					if (val6 != null)
					{
						double num2 = ((val6.RectangleWidth > 0.0) ? val6.RectangleWidth : val6.HorizontalWidth);
						CADTextInfo val7 = new CADTextInfo
						{
							Content = (val6.PlainText ?? val6.Value)
						};
						val5 = val6.InsertPoint;
						val7.X = ((XYZ)val5).X * conversionFactorToMM;
						val5 = val6.InsertPoint;
						val7.Y = ((XYZ)val5).Y * conversionFactorToMM;
						val5 = val6.InsertPoint;
						val7.Z = ((XYZ)val5).Z * conversionFactorToMM;
						val7.LayerName = ((TableEntry)((Entity)val6).Layer).Name;
						val7.Height = val6.Height * conversionFactorToMM;
						val7.Width = num2 * conversionFactorToMM;
						val7.Rotation = val6.Rotation;
						val7.HorizontalAlignment = smethod_0(val6.AttachmentPoint);
						val7.VerticalAlignment = smethod_1(val6.AttachmentPoint);
						list.Add(val7);
					}
				}
				catch
				{
				}
			}
			foreach (Entity entity2 in val.Entities)
			{
				try
				{
					Insert val8 = (Insert)(object)((entity2 is Insert) ? entity2 : null);
					if (val8 == null)
					{
						continue;
					}
					val5 = val8.InsertPoint;
					double x = ((XYZ)val5).X;
					val5 = val8.InsertPoint;
					double y = ((XYZ)val5).Y;
					val5 = val8.InsertPoint;
					double z = ((XYZ)val5).Z;
					double xScale = val8.XScale;
					double yScale = val8.YScale;
					double zScale = val8.ZScale;
					double rotation = val8.Rotation;
					BlockRecord block = val8.Block;
					foreach (Entity entity3 in block.Entities)
					{
						try
						{
							TextEntity val9 = (TextEntity)(object)((entity3 is TextEntity) ? entity3 : null);
							if (val9 != null)
							{
								val5 = val9.InsertPoint;
								double x2 = ((XYZ)val5).X;
								val5 = val9.InsertPoint;
								double y2 = ((XYZ)val5).Y;
								val5 = val9.InsertPoint;
								(double X, double Y, double Z) tuple = smethod_2(x2, y2, ((XYZ)val5).Z, x, y, z, xScale, yScale, zScale, rotation);
								double item = tuple.X;
								double item2 = tuple.Y;
								double item3 = tuple.Z;
								double num3 = (double)val9.Value.Length * val9.Height * val9.WidthFactor;
								list.Add(new CADTextInfo
								{
									Content = val9.Value,
									X = item * conversionFactorToMM,
									Y = item2 * conversionFactorToMM,
									Z = item3 * conversionFactorToMM,
									LayerName = ((TableEntry)((Entity)val9).Layer).Name,
									Height = val9.Height * xScale * conversionFactorToMM,
									Width = num3 * xScale * conversionFactorToMM,
									Rotation = val9.Rotation + rotation,
									HorizontalAlignment = ((object)val9.HorizontalAlignment/*cast due to constrained. prefix*/).ToString(),
									VerticalAlignment = ((object)val9.VerticalAlignment/*cast due to constrained. prefix*/).ToString(),
									BlockName = ((TableEntry)block).Name
								});
								continue;
							}
							MText val10 = (MText)(object)((entity3 is MText) ? entity3 : null);
							if (val10 != null)
							{
								val5 = val10.InsertPoint;
								double x3 = ((XYZ)val5).X;
								val5 = val10.InsertPoint;
								double y3 = ((XYZ)val5).Y;
								val5 = val10.InsertPoint;
								(double X, double Y, double Z) tuple2 = smethod_2(x3, y3, ((XYZ)val5).Z, x, y, z, xScale, yScale, zScale, rotation);
								double item4 = tuple2.X;
								double item5 = tuple2.Y;
								double item6 = tuple2.Z;
								double num4 = ((val10.RectangleWidth > 0.0) ? val10.RectangleWidth : val10.HorizontalWidth);
								list.Add(new CADTextInfo
								{
									Content = (val10.PlainText ?? val10.Value),
									X = item4 * conversionFactorToMM,
									Y = item5 * conversionFactorToMM,
									Z = item6 * conversionFactorToMM,
									LayerName = ((TableEntry)((Entity)val10).Layer).Name,
									Height = val10.Height * xScale * conversionFactorToMM,
									Width = num4 * xScale * conversionFactorToMM,
									Rotation = val10.Rotation + rotation,
									HorizontalAlignment = smethod_0(val10.AttachmentPoint),
									VerticalAlignment = smethod_1(val10.AttachmentPoint),
									BlockName = ((TableEntry)block).Name
								});
							}
						}
						catch
						{
						}
					}
				}
				catch
				{
				}
			}
			val2.Texts = list;
			List<CADBlockInfo> list2 = new List<CADBlockInfo>();
			foreach (Entity entity4 in val.Entities)
			{
				try
				{
					Insert val11 = (Insert)(object)((entity4 is Insert) ? entity4 : null);
					if (val11 != null)
					{
						CADBlockInfo val12 = new CADBlockInfo
						{
							Name = ((TableEntry)val11.Block).Name
						};
						val5 = val11.InsertPoint;
						val12.X = ((XYZ)val5).X * conversionFactorToMM;
						val5 = val11.InsertPoint;
						val12.Y = ((XYZ)val5).Y * conversionFactorToMM;
						val5 = val11.InsertPoint;
						val12.Z = ((XYZ)val5).Z * conversionFactorToMM;
						val12.LayerName = ((TableEntry)((Entity)val11).Layer).Name;
						val12.ScaleX = val11.XScale;
						val12.ScaleY = val11.YScale;
						val12.ScaleZ = val11.ZScale;
						val12.Rotation = val11.Rotation;
						list2.Add(val12);
					}
				}
				catch
				{
				}
			}
			val2.Blocks = list2;
			List<CADLineInfo> list3 = new List<CADLineInfo>();
			XY location;
			foreach (Entity entity5 in val.Entities)
			{
				try
				{
					Line val13 = (Line)(object)((entity5 is Line) ? entity5 : null);
					if (val13 != null)
					{
						CADLineInfo val14 = new CADLineInfo();
						val5 = val13.StartPoint;
						val14.StartX = ((XYZ)val5).X * conversionFactorToMM;
						val5 = val13.StartPoint;
						val14.StartY = ((XYZ)val5).Y * conversionFactorToMM;
						val5 = val13.StartPoint;
						val14.StartZ = ((XYZ)val5).Z * conversionFactorToMM;
						val5 = val13.EndPoint;
						val14.EndX = ((XYZ)val5).X * conversionFactorToMM;
						val5 = val13.EndPoint;
						val14.EndY = ((XYZ)val5).Y * conversionFactorToMM;
						val5 = val13.EndPoint;
						val14.EndZ = ((XYZ)val5).Z * conversionFactorToMM;
						val14.LayerName = ((TableEntry)((Entity)val13).Layer).Name;
						val14.LineType = "Line";
						list3.Add(val14);
						continue;
					}
					LwPolyline val15 = (LwPolyline)(object)((entity5 is LwPolyline) ? entity5 : null);
					if (val15 != null)
					{
						for (int num5 = 0; num5 < val15.Vertices.Count - 1; num5++)
						{
							LwPolyline.Vertex val16 = val15.Vertices[num5];
							LwPolyline.Vertex val17 = val15.Vertices[num5 + 1];
							CADLineInfo val18 = new CADLineInfo();
							location = val16.Location;
							val18.StartX = ((XY)location).X * conversionFactorToMM;
							location = val16.Location;
							val18.StartY = ((XY)location).Y * conversionFactorToMM;
							val18.StartZ = 0.0;
							location = val17.Location;
							val18.EndX = ((XY)location).X * conversionFactorToMM;
							location = val17.Location;
							val18.EndY = ((XY)location).Y * conversionFactorToMM;
							val18.EndZ = 0.0;
							val18.LayerName = ((TableEntry)((Entity)val15).Layer).Name;
							val18.LineType = "LwPolyline";
							list3.Add(val18);
						}
						continue;
					}
					Polyline3D val19 = (Polyline3D)(object)((entity5 is Polyline3D) ? entity5 : null);
					if (val19 != null)
					{
						for (int num6 = 0; num6 < ((CadObjectCollection<Vertex3D>)(object)((Polyline<Vertex3D>)(object)val19).Vertices).Count - 1; num6++)
						{
							Vertex3D val20 = ((CadObjectCollection<Vertex3D>)(object)((Polyline<Vertex3D>)(object)val19).Vertices)[num6];
							Vertex3D val21 = ((CadObjectCollection<Vertex3D>)(object)((Polyline<Vertex3D>)(object)val19).Vertices)[num6 + 1];
							CADLineInfo val22 = new CADLineInfo();
							val5 = ((Vertex)val20).Location;
							val22.StartX = ((XYZ)val5).X * conversionFactorToMM;
							val5 = ((Vertex)val20).Location;
							val22.StartY = ((XYZ)val5).Y * conversionFactorToMM;
							val5 = ((Vertex)val20).Location;
							val22.StartZ = ((XYZ)val5).Z * conversionFactorToMM;
							val5 = ((Vertex)val21).Location;
							val22.EndX = ((XYZ)val5).X * conversionFactorToMM;
							val5 = ((Vertex)val21).Location;
							val22.EndY = ((XYZ)val5).Y * conversionFactorToMM;
							val5 = ((Vertex)val21).Location;
							val22.EndZ = ((XYZ)val5).Z * conversionFactorToMM;
							val22.LayerName = ((TableEntry)((Entity)val19).Layer).Name;
							val22.LineType = "Polyline3D";
							list3.Add(val22);
						}
						continue;
					}
					Arc val23 = (Arc)(object)((entity5 is Arc) ? entity5 : null);
					if (val23 != null)
					{
						XYZ center = ((Circle)val23).Center;
						double radius = ((Circle)val23).Radius;
						double startAngle = val23.StartAngle;
						double endAngle = val23.EndAngle;
						double num7 = ((XYZ)center).X + radius * Math.Cos(startAngle);
						double num8 = ((XYZ)center).Y + radius * Math.Sin(startAngle);
						double z2 = ((XYZ)center).Z;
						double num9 = ((XYZ)center).X + radius * Math.Cos(endAngle);
						double num10 = ((XYZ)center).Y + radius * Math.Sin(endAngle);
						double z3 = ((XYZ)center).Z;
						list3.Add(new CADLineInfo
						{
							StartX = num7 * conversionFactorToMM,
							StartY = num8 * conversionFactorToMM,
							StartZ = z2 * conversionFactorToMM,
							EndX = num9 * conversionFactorToMM,
							EndY = num10 * conversionFactorToMM,
							EndZ = z3 * conversionFactorToMM,
							LayerName = ((TableEntry)((Entity)val23).Layer).Name,
							LineType = "Arc"
						});
					}
					else
					{
						Circle val24 = (Circle)(object)((entity5 is Circle) ? entity5 : null);
						if (val24 != null)
						{
							CADLineInfo val25 = new CADLineInfo();
							val5 = val24.Center;
							val25.StartX = (((XYZ)val5).X - val24.Radius) * conversionFactorToMM;
							val5 = val24.Center;
							val25.StartY = ((XYZ)val5).Y * conversionFactorToMM;
							val5 = val24.Center;
							val25.StartZ = ((XYZ)val5).Z * conversionFactorToMM;
							val5 = val24.Center;
							val25.EndX = (((XYZ)val5).X + val24.Radius) * conversionFactorToMM;
							val5 = val24.Center;
							val25.EndY = ((XYZ)val5).Y * conversionFactorToMM;
							val5 = val24.Center;
							val25.EndZ = ((XYZ)val5).Z * conversionFactorToMM;
							val25.LayerName = ((TableEntry)((Entity)val24).Layer).Name;
							val25.LineType = "Circle";
							list3.Add(val25);
						}
					}
				}
				catch
				{
				}
			}
			foreach (Entity entity6 in val.Entities)
			{
				try
				{
					Insert val26 = (Insert)(object)((entity6 is Insert) ? entity6 : null);
					if (val26 == null)
					{
						continue;
					}
					val5 = val26.InsertPoint;
					double x4 = ((XYZ)val5).X;
					val5 = val26.InsertPoint;
					double y4 = ((XYZ)val5).Y;
					val5 = val26.InsertPoint;
					double z4 = ((XYZ)val5).Z;
					double xScale2 = val26.XScale;
					double yScale2 = val26.YScale;
					double zScale2 = val26.ZScale;
					double rotation2 = val26.Rotation;
					BlockRecord block2 = val26.Block;
					foreach (Entity entity7 in block2.Entities)
					{
						try
						{
							Line val27 = (Line)(object)((entity7 is Line) ? entity7 : null);
							if (val27 != null)
							{
								val5 = val27.StartPoint;
								double x5 = ((XYZ)val5).X;
								val5 = val27.StartPoint;
								double y5 = ((XYZ)val5).Y;
								val5 = val27.StartPoint;
								(double X, double Y, double Z) tuple3 = smethod_2(x5, y5, ((XYZ)val5).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
								double item7 = tuple3.X;
								double item8 = tuple3.Y;
								double item9 = tuple3.Z;
								val5 = val27.EndPoint;
								double x6 = ((XYZ)val5).X;
								val5 = val27.EndPoint;
								double y6 = ((XYZ)val5).Y;
								val5 = val27.EndPoint;
								var (num11, num12, num13) = smethod_2(x6, y6, ((XYZ)val5).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
								list3.Add(new CADLineInfo
								{
									StartX = item7 * conversionFactorToMM,
									StartY = item8 * conversionFactorToMM,
									StartZ = item9 * conversionFactorToMM,
									EndX = num11 * conversionFactorToMM,
									EndY = num12 * conversionFactorToMM,
									EndZ = num13 * conversionFactorToMM,
									LayerName = ((TableEntry)((Entity)val27).Layer).Name,
									LineType = "Line",
									BlockName = ((TableEntry)block2).Name
								});
								continue;
							}
							LwPolyline val28 = (LwPolyline)(object)((entity7 is LwPolyline) ? entity7 : null);
							if (val28 != null)
							{
								for (int num14 = 0; num14 < val28.Vertices.Count - 1; num14++)
								{
									LwPolyline.Vertex val29 = val28.Vertices[num14];
									LwPolyline.Vertex val30 = val28.Vertices[num14 + 1];
									location = (XY)((XY)(val29.Location));
									double x7 = ((XY)location).X;
									location = (XY)((XY)(val29.Location));
									(double X, double Y, double Z) tuple5 = smethod_2(x7, ((XY)location).Y, 0.0, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
									double item10 = tuple5.X;
									double item11 = tuple5.Y;
									double item12 = tuple5.Z;
									location = (XY)((XY)(val30.Location));
									double x8 = ((XY)location).X;
									location = (XY)((XY)(val30.Location));
									var (num15, num16, num17) = smethod_2(x8, ((XY)location).Y, 0.0, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
									list3.Add(new CADLineInfo
									{
										StartX = item10 * conversionFactorToMM,
										StartY = item11 * conversionFactorToMM,
										StartZ = item12 * conversionFactorToMM,
										EndX = num15 * conversionFactorToMM,
										EndY = num16 * conversionFactorToMM,
										EndZ = num17 * conversionFactorToMM,
										LayerName = ((TableEntry)((Entity)val28).Layer).Name,
										LineType = "LwPolyline",
										BlockName = ((TableEntry)block2).Name
									});
								}
								continue;
							}
							Polyline3D val31 = (Polyline3D)(object)((entity7 is Polyline3D) ? entity7 : null);
							if (val31 != null)
							{
								for (int num18 = 0; num18 < ((CadObjectCollection<Vertex3D>)(object)((Polyline<Vertex3D>)(object)val31).Vertices).Count - 1; num18++)
								{
									Vertex3D val32 = ((CadObjectCollection<Vertex3D>)(object)((Polyline<Vertex3D>)(object)val31).Vertices)[num18];
									Vertex3D val33 = ((CadObjectCollection<Vertex3D>)(object)((Polyline<Vertex3D>)(object)val31).Vertices)[num18 + 1];
									val5 = ((Vertex)val32).Location;
									double x9 = ((XYZ)val5).X;
									val5 = ((Vertex)val32).Location;
									double y7 = ((XYZ)val5).Y;
									val5 = ((Vertex)val32).Location;
									(double X, double Y, double Z) tuple7 = smethod_2(x9, y7, ((XYZ)val5).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
									double item13 = tuple7.X;
									double item14 = tuple7.Y;
									double item15 = tuple7.Z;
									val5 = ((Vertex)val33).Location;
									double x10 = ((XYZ)val5).X;
									val5 = ((Vertex)val33).Location;
									double y8 = ((XYZ)val5).Y;
									val5 = ((Vertex)val33).Location;
									var (num19, num20, num21) = smethod_2(x10, y8, ((XYZ)val5).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
									list3.Add(new CADLineInfo
									{
										StartX = item13 * conversionFactorToMM,
										StartY = item14 * conversionFactorToMM,
										StartZ = item15 * conversionFactorToMM,
										EndX = num19 * conversionFactorToMM,
										EndY = num20 * conversionFactorToMM,
										EndZ = num21 * conversionFactorToMM,
										LayerName = ((TableEntry)((Entity)val31).Layer).Name,
										LineType = "Polyline3D",
										BlockName = ((TableEntry)block2).Name
									});
								}
								continue;
							}
							Arc val34 = (Arc)(object)((entity7 is Arc) ? entity7 : null);
							if (val34 != null)
							{
								XYZ center2 = ((Circle)val34).Center;
								double radius2 = ((Circle)val34).Radius;
								double startAngle2 = val34.StartAngle;
								double endAngle2 = val34.EndAngle;
								var (num22, num23, num24) = smethod_2(((XYZ)center2).X + radius2 * Math.Cos(startAngle2), ((XYZ)center2).Y + radius2 * Math.Sin(startAngle2), ((XYZ)center2).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
								var (num25, num26, num27) = smethod_2(((XYZ)center2).X + radius2 * Math.Cos(endAngle2), ((XYZ)center2).Y + radius2 * Math.Sin(endAngle2), ((XYZ)center2).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
								list3.Add(new CADLineInfo
								{
									StartX = num22 * conversionFactorToMM,
									StartY = num23 * conversionFactorToMM,
									StartZ = num24 * conversionFactorToMM,
									EndX = num25 * conversionFactorToMM,
									EndY = num26 * conversionFactorToMM,
									EndZ = num27 * conversionFactorToMM,
									LayerName = ((TableEntry)((Entity)val34).Layer).Name,
									LineType = "Arc",
									BlockName = ((TableEntry)block2).Name
								});
								continue;
							}
							Circle val35 = (Circle)(object)((entity7 is Circle) ? entity7 : null);
							if (val35 != null)
							{
								val5 = val35.Center;
								double double_ = ((XYZ)val5).X - val35.Radius;
								val5 = val35.Center;
								double y9 = ((XYZ)val5).Y;
								val5 = val35.Center;
								(double X, double Y, double Z) tuple11 = smethod_2(double_, y9, ((XYZ)val5).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
								double item16 = tuple11.X;
								double item17 = tuple11.Y;
								double item18 = tuple11.Z;
								val5 = val35.Center;
								double double_2 = ((XYZ)val5).X + val35.Radius;
								val5 = val35.Center;
								double y10 = ((XYZ)val5).Y;
								val5 = val35.Center;
								var (num28, num29, num30) = smethod_2(double_2, y10, ((XYZ)val5).Z, x4, y4, z4, xScale2, yScale2, zScale2, rotation2);
								list3.Add(new CADLineInfo
								{
									StartX = item16 * conversionFactorToMM,
									StartY = item17 * conversionFactorToMM,
									StartZ = item18 * conversionFactorToMM,
									EndX = num28 * conversionFactorToMM,
									EndY = num29 * conversionFactorToMM,
									EndZ = num30 * conversionFactorToMM,
									LayerName = ((TableEntry)((Entity)val35).Layer).Name,
									LineType = "Circle",
									BlockName = ((TableEntry)block2).Name
								});
							}
						}
						catch
						{
						}
					}
				}
				catch
				{
				}
			}
			val2.Lines = list3;
			Logger.Info("[CADFileService] 成功解析 CAD 文件: " + filePath);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(9, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("  - 图层数: ");
			defaultInterpolatedStringHandler2.AppendFormatted(val2.Layers.Count);
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("  - 文字数: ");
			defaultInterpolatedStringHandler3.AppendFormatted(val2.Texts.Count);
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(9, 1);
			defaultInterpolatedStringHandler4.AppendLiteral("  - 图块数: ");
			defaultInterpolatedStringHandler4.AppendFormatted(val2.Blocks.Count);
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(9, 1);
			defaultInterpolatedStringHandler5.AppendLiteral("  - 线条数: ");
			defaultInterpolatedStringHandler5.AppendFormatted(val2.Lines.Count);
			Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
			return val2;
		}
		catch (Exception ex2)
		{
			Logger.Error("[CADFileService] ParseCADFile 失败 (" + filePath + "): " + ex2.Message);
			return null;
		}
	}

	CADFileData? ICADFileService.ParseImportInstance(object importInstance, object document)
	{
		Logger.Warning("[CADFileService] Core 层无法直接解析 ImportInstance，请使用适配层的实现");
		return null;
	}

	CADFileData? ICADFileService.FilterByLayer(object importInstance, object document, string layerName)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		try
		{
			CADFileData val = ((ICADFileService)this).ParseImportInstance(importInstance, document);
			if (val == null)
			{
				return null;
			}
			CADFileData val2 = new CADFileData
			{
				FileInfo = val.FileInfo,
				Layers = val.Layers.Where((CADLayerInfo cadlayerInfo_0) => cadlayerInfo_0.Name == layerName).ToList(),
				Texts = val.Texts.Where((CADTextInfo cadtextInfo_0) => cadtextInfo_0.LayerName == layerName).ToList(),
				Blocks = val.Blocks.Where((CADBlockInfo cadblockInfo_0) => cadblockInfo_0.LayerName == layerName).ToList(),
				Lines = val.Lines.Where((CADLineInfo cadlineInfo_0) => cadlineInfo_0.LayerName == layerName).ToList()
			};
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 4);
			defaultInterpolatedStringHandler.AppendLiteral("[CADFileService] 过滤图层 '");
			defaultInterpolatedStringHandler.AppendFormatted(layerName);
			defaultInterpolatedStringHandler.AppendLiteral("': ");
			defaultInterpolatedStringHandler.AppendLiteral("文字=");
			defaultInterpolatedStringHandler.AppendFormatted(val2.Texts.Count);
			defaultInterpolatedStringHandler.AppendLiteral(", 图块=");
			defaultInterpolatedStringHandler.AppendFormatted(val2.Blocks.Count);
			defaultInterpolatedStringHandler.AppendLiteral(", 线条=");
			defaultInterpolatedStringHandler.AppendFormatted(val2.Lines.Count);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return val2;
		}
		catch (Exception ex)
		{
			Logger.Error("[CADFileService] FilterByLayer 失败: " + ex.Message);
			return null;
		}
	}

	List<CADFileData> ICADFileService.ParseMultipleImportInstances(IEnumerable<object> importInstances, object document)
	{
		List<CADFileData> list = new List<CADFileData>();
		foreach (object importInstance in importInstances)
		{
			try
			{
				CADFileData val = ((ICADFileService)this).ParseImportInstance(importInstance, document);
				if (val != null)
				{
					list.Add(val);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[CADFileService] 批量解析失败: " + ex.Message);
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[CADFileService] 批量解析完成: 成功 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral("/");
		defaultInterpolatedStringHandler.AppendFormatted(importInstances.Count());
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		return list;
	}

	bool ICADFileService.IsDwgFile(string filePath)
	{
		try
		{
			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				return Path.GetExtension(filePath).ToLower() == ".dwg";
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("[CADFileService] IsDwgFile 检查失败: " + ex.Message);
			return false;
		}
	}

	[Obsolete("ACadSharp 直接支持 DWG 文件，无需转换")]
	string? ICADFileService.ConvertDwgToDxf(string dwgFilePath, string? outputFolder = null)
	{
		Logger.Warning("[CADFileService] ACadSharp 直接支持 DWG 文件，无需转换");
		return null;
	}

	[Obsolete("ACadSharp 直接支持 DWG 文件，无需转换工具")]
	DwgConverterStatus ICADFileService.GetConverterStatus()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		return new DwgConverterStatus
		{
			HasOdaConverter = false,
			HasAutoCAD = false,
			SupportedMethods = new List<string> { "直接读取 DWG 文件（ACadSharp）" },
			RecommendedMethod = "ACadSharp",
			Message = "ACadSharp 直接支持 DWG 和 DXF 格式，无需转换工具，开箱即用。"
		};
	}

	List<string> ICADFileService.GetLayersFromCADFile(string cadFilePath)
	{
		try
		{
			if (!File.Exists(cadFilePath))
			{
				Logger.Error("[CADFileService] 文件不存在: " + cadFilePath);
				return new List<string>();
			}
			string text = Path.GetExtension(cadFilePath).ToLower();
			if (text != ".dwg" && text != ".dxf")
			{
				Logger.Error("[CADFileService] 不支持的文件格式: " + text);
				return new List<string>();
			}
			Logger.Info("[CADFileService] 开始读取 CAD 文件图层: " + cadFilePath);
			CadDocument val = null;
			try
			{
				val = ((!(text == ".dwg")) ? DxfReader.Read(cadFilePath, (NotificationEventHandler)null) : DwgReader.Read(cadFilePath, (NotificationEventHandler)null));
			}
			catch (Exception ex)
			{
				Logger.Error("[CADFileService] ACadSharp 读取文件失败: " + ex.Message);
				return new List<string>();
			}
			if (val == null)
			{
				Logger.Error("[CADFileService] 无法加载 CAD 文件: " + cadFilePath);
				return new List<string>();
			}
			List<string> list = (from layer_0 in (IEnumerable<Layer>)val.Layers
				select ((TableEntry)layer_0).Name into string_0
				orderby string_0
				select string_0).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CADFileService] 成功读取 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个图层");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex2)
		{
			Logger.Error("[CADFileService] GetLayersFromCADFile 失败 (" + cadFilePath + "): " + ex2.Message);
			return new List<string>();
		}
	}

	private static string smethod_0(AttachmentPointType attachmentPointType_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Expected I4, but got Unknown
		switch ((int)attachmentPointType_0)
		{
		default:
			return "Left";
		case 1:
		case 4:
		case 7:
			return "Left";
		case 2:
		case 5:
		case 8:
			return "Center";
		case 3:
		case 6:
		case 9:
			return "Right";
		}
	}

	private static string smethod_1(AttachmentPointType attachmentPointType_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Expected I4, but got Unknown
		switch ((int)attachmentPointType_0)
		{
		default:
			return "Top";
		case 1:
		case 2:
		case 3:
			return "Top";
		case 4:
		case 5:
		case 6:
			return "Middle";
		case 7:
		case 8:
		case 9:
			return "Bottom";
		}
	}

	private static (double X, double Y, double Z) smethod_2(double double_0, double double_1, double double_2, double double_3, double double_4, double double_5, double double_6, double double_7, double double_8, double double_9)
	{
		double num = double_0 * double_6;
		double num2 = double_1 * double_7;
		double num3 = double_2 * double_8;
		double num4 = Math.Cos(double_9);
		double num5 = Math.Sin(double_9);
		double num6 = num * num4 - num2 * num5;
		double num7 = num * num5 + num2 * num4;
		double num8 = num3;
		double item = num6 + double_3;
		double item2 = num7 + double_4;
		double item3 = num8 + double_5;
		return (X: item, Y: item2, Z: item3);
	}
}
