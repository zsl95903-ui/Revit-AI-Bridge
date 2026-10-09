using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Infrastructure;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using ns6;

using Document = Autodesk.Revit.DB.Document;

namespace RevitAi.Revit.Services;

internal sealed class RoadModelingService : IRoadModelingService
{
	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass3_0
	{
		public SubgradeLayerData layer;

		public Func<SubgradeLayerData, bool> _003C_003E9__3;

		public Func<SubgradeLayerData, bool> _003C_003E9__4;

		internal bool _003CCreateRoadModelAsync_003Eb__3(SubgradeLayerData l)
		{
			return l.Order < layer.Order;
		}

		internal bool _003CCreateRoadModelAsync_003Eb__4(SubgradeLayerData l)
		{
			return l.Order < layer.Order;
		}
	}

	[CompilerGenerated]
	public sealed class _003C_003Ec__DisplayClass4_0
	{
		public string familyName;

		internal bool _003CFindOrLoadFamilyAsync_003Eb__0(Family f)
		{
			return ((Element)f).Name.Equals(familyName, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	public sealed class _003CCreateAncillaryStructureAsync_003Ed__20 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<Result<RoadModelingResult>> _003C_003Et__builder;

		public object document;

		public RoadProject project;

		public RoadModelingService _003C_003E4__this;

		private Document _003Cdoc_003E5__1;

		private RoadModelingResult _003Cresult_003E5__2;

		private Dictionary<string, int> _003CstructureInstanceCounts_003E5__3;

		private List<string> _003CmissingFamilies_003E5__4;

		private InfrastructureService _003CinfrastructureService_003E5__5;

		private RoadProjectService _003CroadProjectService_003E5__6;

		private Result<List<RoadCenterlinePoint3D>> _003CcalcResult_003E5__7;

		private Transaction _003Ctransaction_003E5__8;

		private List<AncillaryStructureData> _003CsortedStructures_003E5__9;

		private List<AncillaryStructureData>.Enumerator _003C_003Es__10;

		private AncillaryStructureData _003Cstructure_003E5__11;

		private string _003CfamilyName_003E5__12;

		private Family _003Cfamily_003E5__13;

		private string _003CtypeName_003E5__14;

		private FamilySymbol _003Csymbol_003E5__15;

		private Family _003C_003Es__16;

		private List<AncillaryStructureLayoutSegmentData>.Enumerator _003C_003Es__17;

		private AncillaryStructureLayoutSegmentData _003ClayoutSegment_003E5__18;

		private List<(double StartKm, double EndKm)> _003CsubSegments_003E5__19;

		private AncillaryPositionType _003CpositionType_003E5__20;

		private List<string> _003CcreatedIds_003E5__21;

		private string _003CmissingHint_003E5__22;

		private Exception _003Cex_003E5__23;

		private Exception _003Cex_003E5__24;

		private TaskAwaiter<Family?> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Expected O, but got Unknown
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Expected O, but got Unknown
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0843: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			if (num == 0)
			{
			}
			Result<RoadModelingResult> result;
			try
			{
				if (num == 0)
				{
					goto IL_01ec;
				}
				object obj = document;
				_003Cdoc_003E5__1 = (Document)((obj is Document) ? obj : null);
				if (_003Cdoc_003E5__1 == null)
				{
					result = Result<RoadModelingResult>.Failure("文档类型无效");
				}
				else if (project == null)
				{
					result = Result<RoadModelingResult>.Failure("道路项目不能为空");
				}
				else
				{
					AncillaryStructuresConfiguration ancillaryStructuresConfiguration = project.AncillaryStructuresConfiguration;
					if (((ancillaryStructuresConfiguration != null) ? ancillaryStructuresConfiguration.Structures : null) == null || project.AncillaryStructuresConfiguration.Structures.Count == 0)
					{
						result = Result<RoadModelingResult>.Failure("项目未配置附属结构");
					}
					else
					{
						if (project.Centerline3DPoints != null && project.Centerline3DPoints.Count != 0)
						{
							goto IL_0185;
						}
						_003C_003E4__this._logger.Info("[RoadModelingService] 附属结构建模：项目未生成三维曲线，开始自动计算");
						_003CinfrastructureService_003E5__5 = new InfrastructureService();
						_003CroadProjectService_003E5__6 = new RoadProjectService((IInfrastructureService)(object)_003CinfrastructureService_003E5__5);
						_003CcalcResult_003E5__7 = _003CroadProjectService_003E5__6.Calculate3DCenterline(project);
						if (_003CcalcResult_003E5__7.IsSuccess && _003CcalcResult_003E5__7.Value != null && _003CcalcResult_003E5__7.Value.Count != 0)
						{
							project.Centerline3DPoints = _003CcalcResult_003E5__7.Value;
							_003CinfrastructureService_003E5__5 = null;
							_003CroadProjectService_003E5__6 = null;
							_003CcalcResult_003E5__7 = null;
							goto IL_0185;
						}
						result = Result<RoadModelingResult>.Failure("计算三维曲线失败: " + _003CcalcResult_003E5__7.Error);
					}
				}
				goto end_IL_000b;
				IL_01ec:
				try
				{
					if (num != 0)
					{
						_003Ctransaction_003E5__8.Start();
					}
					try
					{
						if (num != 0)
						{
							_003CsortedStructures_003E5__9 = project.AncillaryStructuresConfiguration.Structures.OrderBy((AncillaryStructureData s) => s.Order).ToList();
							_003C_003Es__10 = _003CsortedStructures_003E5__9.GetEnumerator();
						}
						try
						{
							if (num != 0)
							{
								goto IL_05b4;
							}
							TaskAwaiter<Family> awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(TaskAwaiter<Family>);
							num = -1;
							_003C_003E1__state = -1;
							goto IL_065f;
							IL_05b4:
							while (_003C_003Es__10.MoveNext())
							{
								_003Cstructure_003E5__11 = _003C_003Es__10.Current;
								if (!_003Cstructure_003E5__11.IsEnabled)
								{
									_003C_003E4__this._logger.Info("[RoadModelingService] 跳过禁用的附属结构: " + _003Cstructure_003E5__11.Name);
									continue;
								}
								if (_003Cstructure_003E5__11.LayoutSegments == null || _003Cstructure_003E5__11.LayoutSegments.Count == 0)
								{
									_003C_003E4__this._logger.Warning("[RoadModelingService] 附属结构 " + _003Cstructure_003E5__11.Name + " 没有布置段，跳过");
									continue;
								}
								_003CfamilyName_003E5__12 = GetFamilyNameByStructureType(_003Cstructure_003E5__11.StructureType);
								awaiter = _003C_003E4__this.FindOrLoadFamilyAsync(_003Cdoc_003E5__1, _003CfamilyName_003E5__12).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = 0;
									_003C_003E1__state = 0;
									_003C_003Eu__1 = awaiter;
									_003CCreateAncillaryStructureAsync_003Ed__20 stateMachine = this;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
									return;
								}
								goto IL_065f;
							}
							goto end_IL_0250;
							IL_065f:
							_003C_003Es__16 = awaiter.GetResult();
							_003Cfamily_003E5__13 = _003C_003Es__16;
							_003C_003Es__16 = null;
							if (_003Cfamily_003E5__13 != null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
								defaultInterpolatedStringHandler.AppendFormatted(_003Cstructure_003E5__11.Name);
								defaultInterpolatedStringHandler.AppendLiteral("_");
								defaultInterpolatedStringHandler.AppendFormatted(_003Cstructure_003E5__11.Width);
								defaultInterpolatedStringHandler.AppendLiteral("x");
								defaultInterpolatedStringHandler.AppendFormatted(_003Cstructure_003E5__11.Height);
								_003CtypeName_003E5__14 = defaultInterpolatedStringHandler.ToStringAndClear();
								_003Csymbol_003E5__15 = _003C_003E4__this.GetOrCreateFamilyTypeForAncillaryStructure(_003Cdoc_003E5__1, _003Cfamily_003E5__13, _003CtypeName_003E5__14, _003Cstructure_003E5__11);
								if (_003Csymbol_003E5__15 == null)
								{
									_003C_003E4__this._logger.Warning("[RoadModelingService] 无法创建族类型: " + _003CtypeName_003E5__14);
								}
								else
								{
									RoadModelingResult obj2 = _003Cresult_003E5__2;
									int createdTypeCount = obj2.CreatedTypeCount;
									obj2.CreatedTypeCount = createdTypeCount + 1;
									_003CstructureInstanceCounts_003E5__3[_003Cstructure_003E5__11.Name] = 0;
									if (!_003Csymbol_003E5__15.IsActive)
									{
										_003Csymbol_003E5__15.Activate();
										_003Cdoc_003E5__1.Regenerate();
									}
									_003C_003Es__17 = _003Cstructure_003E5__11.LayoutSegments.GetEnumerator();
									try
									{
										while (_003C_003Es__17.MoveNext())
										{
											_003ClayoutSegment_003E5__18 = _003C_003Es__17.Current;
											if (!(_003ClayoutSegment_003E5__18.EndStationKm <= _003ClayoutSegment_003E5__18.StartStationKm))
											{
												_003CsubSegments_003E5__19 = _003C_003E4__this.SplitLayoutSegment(_003ClayoutSegment_003E5__18);
												if (_003CsubSegments_003E5__19.Count != 0)
												{
													_003CpositionType_003E5__20 = _003Cstructure_003E5__11.PositionType;
													_003CcreatedIds_003E5__21 = _003C_003E4__this.CreateInstancesForSubSegments(_003Cdoc_003E5__1, project, _003Cstructure_003E5__11, _003CpositionType_003E5__20, _003CsubSegments_003E5__19, _003Csymbol_003E5__15);
													_003CstructureInstanceCounts_003E5__3[_003Cstructure_003E5__11.Name] += _003CcreatedIds_003E5__21.Count;
													_003Cresult_003E5__2.CreatedInstanceIds.AddRange(_003CcreatedIds_003E5__21);
													_003CsubSegments_003E5__19 = null;
													_003CcreatedIds_003E5__21 = null;
													_003ClayoutSegment_003E5__18 = null;
												}
											}
										}
									}
									finally
									{
										if (num < 0)
										{
											((IDisposable)_003C_003Es__17/*cast due to constrained. prefix*/).Dispose();
										}
									}
									_003C_003Es__17 = default(List<AncillaryStructureLayoutSegmentData>.Enumerator);
									_003CfamilyName_003E5__12 = null;
									_003Cfamily_003E5__13 = null;
									_003CtypeName_003E5__14 = null;
									_003Csymbol_003E5__15 = null;
									_003Cstructure_003E5__11 = null;
								}
							}
							else
							{
								_003C_003E4__this._logger.Error("[RoadModelingService] 无法找到或加载族: " + _003CfamilyName_003E5__12, (Exception)null);
								if (!_003CmissingFamilies_003E5__4.Contains(_003CfamilyName_003E5__12))
								{
									_003CmissingFamilies_003E5__4.Add(_003CfamilyName_003E5__12);
								}
							}
							goto IL_05b4;
							end_IL_0250:;
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)_003C_003Es__10/*cast due to constrained. prefix*/).Dispose();
							}
						}
						_003C_003Es__10 = default(List<AncillaryStructureData>.Enumerator);
						_003Cresult_003E5__2.LayerInstanceCounts = _003CstructureInstanceCounts_003E5__3;
						_003Cresult_003E5__2.CreatedInstanceCount = _003Cresult_003E5__2.CreatedInstanceIds.Count;
						_003Cresult_003E5__2.IsSuccess = true;
						if (_003Cresult_003E5__2.CreatedInstanceCount == 0)
						{
							_003CmissingHint_003E5__22 = ((_003CmissingFamilies_003E5__4.Count > 0) ? ("缺失族: " + string.Join(", ", _003CmissingFamilies_003E5__4) + "。请确认族文件已上传到族库。") : "请检查附属结构定义和布置段配置。");
							_003Cresult_003E5__2.Message = "未能创建任何附属结构实例。" + _003CmissingHint_003E5__22;
							_003CmissingHint_003E5__22 = null;
						}
						else
						{
							RoadModelingResult obj3 = _003Cresult_003E5__2;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("成功创建 ");
							defaultInterpolatedStringHandler2.AppendFormatted(_003Cresult_003E5__2.CreatedInstanceCount);
							defaultInterpolatedStringHandler2.AppendLiteral(" 个附属结构实例");
							obj3.Message = defaultInterpolatedStringHandler2.ToStringAndClear();
							if (_003CmissingFamilies_003E5__4.Count > 0)
							{
								RoadModelingResult obj4 = _003Cresult_003E5__2;
								obj4.Message = obj4.Message + "（部分族未找到已跳过: " + string.Join(", ", _003CmissingFamilies_003E5__4) + "）";
							}
						}
						_003Ctransaction_003E5__8.Commit();
						_003C_003E4__this._logger.Info("[RoadModelingService] 附属结构建模完成: " + _003Cresult_003E5__2.Message);
						result = Result<RoadModelingResult>.Success(_003Cresult_003E5__2);
					}
					catch (Exception ex)
					{
						_003Cex_003E5__23 = ex;
						_003Ctransaction_003E5__8.RollBack();
						_003C_003E4__this._logger.Error("[RoadModelingService] 创建附属结构实例失败", _003Cex_003E5__23);
						result = Result<RoadModelingResult>.Failure("创建实例失败: " + _003Cex_003E5__23.Message);
					}
				}
				finally
				{
					if (num < 0 && _003Ctransaction_003E5__8 != null)
					{
						((IDisposable)_003Ctransaction_003E5__8).Dispose();
					}
				}
				goto end_IL_000b;
				IL_0185:
				_003C_003E4__this._logger.Info("[RoadModelingService] 开始创建附属结构模型: 项目=" + project.Name);
				_003Cresult_003E5__2 = new RoadModelingResult();
				_003CstructureInstanceCounts_003E5__3 = new Dictionary<string, int>();
				_003CmissingFamilies_003E5__4 = new List<string>();
				_003Ctransaction_003E5__8 = new Transaction(_003Cdoc_003E5__1, "创建附属结构模型");
				goto IL_01ec;
				end_IL_000b:;
			}
			catch (Exception ex)
			{
				_003Cex_003E5__24 = ex;
				_003C_003E4__this._logger.Error("[RoadModelingService] 创建附属结构模型失败", _003Cex_003E5__24);
				result = Result<RoadModelingResult>.Failure("创建失败: " + _003Cex_003E5__24.Message);
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[CompilerGenerated]
	public sealed class _003CCreateRoadModelAsync_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<Result<RoadModelingResult>> _003C_003Et__builder;

		public object document;

		public RoadProject project;

		public bool splitAtIntegerStations;

		public int integerStationInterval;

		public RoadModelingService _003C_003E4__this;

		private Document _003Cdoc_003E5__1;

		private bool _003CneedRecalculate_003E5__2;

		private RoadModelingResult _003Cresult_003E5__3;

		private Dictionary<string, int> _003ClayerInstanceCounts_003E5__4;

		private string _003CtargetFamilyName_003E5__5;

		private Family _003Cfamily_003E5__6;

		private List<SubgradeLayerData> _003Clayers_003E5__7;

		private List<double> _003CstationPositions_003E5__8;

		private RoadCenterlinePoint3D _003CfirstPoint_003E5__9;

		private InfrastructureService _003CinfrastructureService_003E5__10;

		private RoadProjectService _003CroadProjectService_003E5__11;

		private Result<List<RoadCenterlinePoint3D>> _003CcalcResult_003E5__12;

		private Family _003C_003Es__13;

		private Transaction _003Ctransaction_003E5__14;

		private List<(SubgradeLayerData Layer, FamilySymbol Symbol)> _003CfamilyTypes_003E5__15;

		private List<SubgradeLayerData> _003CsortedLayers_003E5__16;

		private List<SubgradeLayerData>.Enumerator _003C_003Es__17;

		private SubgradeLayerData _003Clayer_003E5__18;

		private string _003CtypeName_003E5__19;

		private FamilySymbol _003Csymbol_003E5__20;

		private IEnumerator<(SubgradeLayerData Layer, FamilySymbol Symbol)> _003C_003Es__21;

		private _003C_003Ec__DisplayClass3_0 _003C_003E8__22;

		private FamilySymbol _003Csymbol_003E5__23;

		private double _003CverticalOffset_003E5__24;

		private double _003CtopOffsetFeet_003E5__25;

		private double _003CbaseWidth_003E5__26;

		private List<FamilyInstanceCreationData> _003CbatchData_003E5__27;

		private List<(double W1, double W2, double W3)> _003CbatchWidths_003E5__28;

		private List<(double A1, double A2, double A3)> _003CbatchAngles_003E5__29;

		private List<(double OA1, double OA2, double OA3)> _003CbatchOffsetAngles_003E5__30;

		private IEnumerator<SubgradeLayerData> _003C_003Es__31;

		private SubgradeLayerData _003CupperLayer_003E5__32;

		private double _003CcumulativeWidthMm_003E5__33;

		private IEnumerator<SubgradeLayerData> _003C_003Es__34;

		private SubgradeLayerData _003CupperLayer_003E5__35;

		private double _003CincrementMm_003E5__36;

		private double _003CslopeWidthMm_003E5__37;

		private int _003Ci_003E5__38;

		private double _003CstartStation_003E5__39;

		private double _003CendStation_003E5__40;

		private IList<XYZ> _003Cpoints_003E5__41;

		private double _003CbaseWidth1_003E5__42;

		private double _003CbaseWidth3_003E5__43;

		private double _003Cw1_003E5__44;

		private double _003Cw3_003E5__45;

		private double _003Cw2_003E5__46;

		private double _003CmidStation_003E5__47;

		private double _003Ca1_003E5__48;

		private double _003Ca2_003E5__49;

		private double _003Ca3_003E5__50;

		private double _003CoffsetAngle1_003E5__51;

		private double _003CoffsetAngle2_003E5__52;

		private double _003CoffsetAngle3_003E5__53;

		private ICollection<ElementId> _003CcreatedIds_003E5__54;

		private int _003CbatchIndex_003E5__55;

		private IEnumerator<ElementId> _003C_003Es__56;

		private ElementId _003Cid_003E5__57;

		private FamilyInstance _003Cinstance_003E5__58;

		private double _003Cw1_003E5__59;

		private double _003Cw2_003E5__60;

		private double _003Cw3_003E5__61;

		private double _003Ca1_003E5__62;

		private double _003Ca2_003E5__63;

		private double _003Ca3_003E5__64;

		private double _003Coa1_003E5__65;

		private double _003Coa2_003E5__66;

		private double _003Coa3_003E5__67;

		private Exception _003Cex_003E5__68;

		private Exception _003Cex_003E5__69;

		private TaskAwaiter<Family?> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_116a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Expected O, but got Unknown
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Invalid comparison between Unknown and I4
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Expected O, but got Unknown
			//IL_1119: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a43: Expected O, but got Unknown
			int num = _003C_003E1__state;
			if (num == 0)
			{
			}
			Result<RoadModelingResult> result;
			try
			{
				TaskAwaiter<Family> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<Family>);
					num = -1;
					_003C_003E1__state = -1;
					goto IL_0381;
				}
				object obj = document;
				_003Cdoc_003E5__1 = (Document)((obj is Document) ? obj : null);
				if (_003Cdoc_003E5__1 == null)
				{
					result = Result<RoadModelingResult>.Failure("文档类型无效");
				}
				else if (project == null)
				{
					result = Result<RoadModelingResult>.Failure("道路项目不能为空");
				}
				else
				{
					_003CneedRecalculate_003E5__2 = false;
					if (project.Centerline3DPoints == null || project.Centerline3DPoints.Count == 0)
					{
						_003CneedRecalculate_003E5__2 = true;
						_003C_003E4__this._logger.Info("[RoadModelingService] 项目未生成三维曲线，开始自动计算");
					}
					else if ((int)project.DataSource == 1)
					{
						_003CfirstPoint_003E5__9 = project.Centerline3DPoints[0];
						if (Math.Abs(_003CfirstPoint_003E5__9.Azimuth) < 1E-06 && Math.Abs(_003CfirstPoint_003E5__9.Curvature) < 1E-06 && project.StationElevationData != null && project.StationElevationData.Count > 1)
						{
							_003CneedRecalculate_003E5__2 = true;
							_003C_003E4__this._logger.Info("[RoadModelingService] 桩号高程表的三维曲线未计算方位角，开始重新计算");
						}
						_003CfirstPoint_003E5__9 = null;
					}
					if (!_003CneedRecalculate_003E5__2)
					{
						goto IL_027d;
					}
					_003CinfrastructureService_003E5__10 = new InfrastructureService();
					_003CroadProjectService_003E5__11 = new RoadProjectService((IInfrastructureService)(object)_003CinfrastructureService_003E5__10);
					_003CcalcResult_003E5__12 = _003CroadProjectService_003E5__11.Calculate3DCenterline(project);
					if (_003CcalcResult_003E5__12.IsSuccess && _003CcalcResult_003E5__12.Value != null && _003CcalcResult_003E5__12.Value.Count != 0)
					{
						project.Centerline3DPoints = _003CcalcResult_003E5__12.Value;
						ILogger logger = _003C_003E4__this._logger;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[RoadModelingService] 自动计算完成，生成了 ");
						defaultInterpolatedStringHandler.AppendFormatted(_003CcalcResult_003E5__12.Value.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个三维点");
						logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						_003CinfrastructureService_003E5__10 = null;
						_003CroadProjectService_003E5__11 = null;
						_003CcalcResult_003E5__12 = null;
						goto IL_027d;
					}
					result = Result<RoadModelingResult>.Failure("计算三维曲线失败: " + _003CcalcResult_003E5__12.Error);
				}
				goto end_IL_000b;
				IL_027d:
				RoadStructureConfiguration roadStructureConfiguration = project.RoadStructureConfiguration;
				if (((roadStructureConfiguration != null) ? roadStructureConfiguration.Layers : null) != null && project.RoadStructureConfiguration.Layers.Count != 0)
				{
					_003C_003E4__this._logger.Info("[RoadModelingService] 开始创建路基路面模型: 项目=" + project.Name);
					_003Cresult_003E5__3 = new RoadModelingResult();
					_003ClayerInstanceCounts_003E5__4 = new Dictionary<string, int>();
					_003CtargetFamilyName_003E5__5 = "AST_R_路基路面_3";
					awaiter = _003C_003E4__this.FindOrLoadFamilyAsync(_003Cdoc_003E5__1, _003CtargetFamilyName_003E5__5).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						_003C_003E1__state = 0;
						_003C_003Eu__1 = awaiter;
						_003CCreateRoadModelAsync_003Ed__3 stateMachine = this;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
						return;
					}
					goto IL_0381;
				}
				result = Result<RoadModelingResult>.Failure("道路项目未配置路面结构");
				goto end_IL_000b;
				IL_0381:
				_003C_003Es__13 = awaiter.GetResult();
				_003Cfamily_003E5__6 = _003C_003Es__13;
				_003C_003Es__13 = null;
				if (_003Cfamily_003E5__6 != null)
				{
					_003C_003E4__this._logger.Info("[RoadModelingService] 族已加载: " + _003CtargetFamilyName_003E5__5);
					_003Clayers_003E5__7 = project.RoadStructureConfiguration.Layers.OrderBy((SubgradeLayerData l) => l.Order).ToList();
					_003CstationPositions_003E5__8 = _003C_003E4__this.CalculateStationPositions(project, splitAtIntegerStations, integerStationInterval);
					ILogger logger2 = _003C_003E4__this._logger;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[RoadModelingService] 计算了 ");
					defaultInterpolatedStringHandler2.AppendFormatted(_003CstationPositions_003E5__8.Count);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个桩号位置");
					logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					_003Ctransaction_003E5__14 = new Transaction(_003Cdoc_003E5__1, "创建路基路面模型");
					try
					{
						_003Ctransaction_003E5__14.Start();
						try
						{
							_003CfamilyTypes_003E5__15 = new List<(SubgradeLayerData, FamilySymbol)>();
							_003CsortedLayers_003E5__16 = _003Clayers_003E5__7.OrderBy((SubgradeLayerData l) => l.Order).ToList();
							_003C_003Es__17 = _003CsortedLayers_003E5__16.GetEnumerator();
							try
							{
								while (_003C_003Es__17.MoveNext())
								{
									_003Clayer_003E5__18 = _003C_003Es__17.Current;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(1, 2);
									defaultInterpolatedStringHandler3.AppendFormatted(_003Clayer_003E5__18.Name);
									defaultInterpolatedStringHandler3.AppendLiteral("_");
									defaultInterpolatedStringHandler3.AppendFormatted(_003Clayer_003E5__18.Height);
									_003CtypeName_003E5__19 = defaultInterpolatedStringHandler3.ToStringAndClear();
									_003Csymbol_003E5__20 = _003C_003E4__this.GetOrCreateFamilyType(_003Cdoc_003E5__1, _003Cfamily_003E5__6, _003CtypeName_003E5__19, _003Clayer_003E5__18, _003CsortedLayers_003E5__16);
									if (_003Csymbol_003E5__20 == null)
									{
										_003C_003E4__this._logger.Warning("[RoadModelingService] 无法创建族类型: " + _003CtypeName_003E5__19);
										continue;
									}
									_003CfamilyTypes_003E5__15.Add((_003Clayer_003E5__18, _003Csymbol_003E5__20));
									RoadModelingResult obj2 = _003Cresult_003E5__3;
									int createdTypeCount = obj2.CreatedTypeCount;
									obj2.CreatedTypeCount = createdTypeCount + 1;
									_003ClayerInstanceCounts_003E5__4[_003Clayer_003E5__18.Name] = 0;
									_003CtypeName_003E5__19 = null;
									_003Csymbol_003E5__20 = null;
									_003Clayer_003E5__18 = null;
								}
							}
							finally
							{
								if (num < 0)
								{
									((IDisposable)_003C_003Es__17/*cast due to constrained. prefix*/).Dispose();
								}
							}
							_003C_003Es__17 = default(List<SubgradeLayerData>.Enumerator);
							ILogger logger3 = _003C_003E4__this._logger;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(31, 1);
							defaultInterpolatedStringHandler4.AppendLiteral("[RoadModelingService] 已创建 ");
							defaultInterpolatedStringHandler4.AppendFormatted(_003CfamilyTypes_003E5__15.Count);
							defaultInterpolatedStringHandler4.AppendLiteral(" 个族类型");
							logger3.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
							_003C_003Es__21 = _003CfamilyTypes_003E5__15.OrderByDescending<(SubgradeLayerData, FamilySymbol), int>(((SubgradeLayerData Layer, FamilySymbol Symbol) x) => x.Layer.Order).GetEnumerator();
							try
							{
								while (_003C_003Es__21.MoveNext())
								{
									_003C_003E8__22 = new _003C_003Ec__DisplayClass3_0();
									(SubgradeLayerData, FamilySymbol) current = _003C_003Es__21.Current;
									_003C_003E8__22.layer = current.Item1;
									_003Csymbol_003E5__23 = current.Item2;
									if (!_003Csymbol_003E5__23.IsActive)
									{
										_003Csymbol_003E5__23.Activate();
										_003Cdoc_003E5__1.Regenerate();
									}
									_003CverticalOffset_003E5__24 = 0.0;
									_003C_003Es__31 = _003Clayers_003E5__7.Where((SubgradeLayerData l) => l.Order < _003C_003E8__22.layer.Order).GetEnumerator();
									try
									{
										while (_003C_003Es__31.MoveNext())
										{
											_003CupperLayer_003E5__32 = _003C_003Es__31.Current;
											_003CverticalOffset_003E5__24 += _003CupperLayer_003E5__32.Height;
											_003CupperLayer_003E5__32 = null;
										}
									}
									finally
									{
										if (num < 0 && _003C_003Es__31 != null)
										{
											_003C_003Es__31.Dispose();
										}
									}
									_003C_003Es__31 = null;
									_003CtopOffsetFeet_003E5__25 = (0.0 - _003CverticalOffset_003E5__24) / 304.8;
									_003CbaseWidth_003E5__26 = 0.0;
									if (_003C_003E8__22.layer.Order == 0)
									{
										_003CbaseWidth_003E5__26 = 0.0;
									}
									else
									{
										_003CcumulativeWidthMm_003E5__33 = 0.0;
										_003C_003Es__34 = _003Clayers_003E5__7.Where((SubgradeLayerData l) => l.Order < _003C_003E8__22.layer.Order).GetEnumerator();
										try
										{
											while (_003C_003Es__34.MoveNext())
											{
												_003CupperLayer_003E5__35 = _003C_003Es__34.Current;
												_003CincrementMm_003E5__36 = (((int)_003CupperLayer_003E5__35.IncrementType == 0) ? (_003CupperLayer_003E5__35.WidthIncrement * 2.0) : _003CupperLayer_003E5__35.WidthIncrement);
												_003CslopeWidthMm_003E5__37 = 2.0 * _003CupperLayer_003E5__35.Height * _003CupperLayer_003E5__35.SlopeRatio;
												_003CcumulativeWidthMm_003E5__33 += _003CincrementMm_003E5__36 + _003CslopeWidthMm_003E5__37;
												_003CupperLayer_003E5__35 = null;
											}
										}
										finally
										{
											if (num < 0 && _003C_003Es__34 != null)
											{
												_003C_003Es__34.Dispose();
											}
										}
										_003C_003Es__34 = null;
										_003CbaseWidth_003E5__26 = _003CcumulativeWidthMm_003E5__33 / 1000.0;
									}
									_003CbatchData_003E5__27 = new List<FamilyInstanceCreationData>();
									_003CbatchWidths_003E5__28 = new List<(double, double, double)>();
									_003CbatchAngles_003E5__29 = new List<(double, double, double)>();
									_003CbatchOffsetAngles_003E5__30 = new List<(double, double, double)>();
									_003Ci_003E5__38 = 0;
									while (_003Ci_003E5__38 < _003CstationPositions_003E5__8.Count - 1)
									{
										_003CstartStation_003E5__39 = _003CstationPositions_003E5__8[_003Ci_003E5__38];
										_003CendStation_003E5__40 = _003CstationPositions_003E5__8[_003Ci_003E5__38 + 1];
										_003Cpoints_003E5__41 = _003C_003E4__this.CalculateAdaptivePoints(project, _003CstartStation_003E5__39, _003CendStation_003E5__40);
										if (_003Cpoints_003E5__41 != null && _003Cpoints_003E5__41.Count == 3)
										{
											_003CbatchData_003E5__27.Add(new FamilyInstanceCreationData(_003Csymbol_003E5__23, _003Cpoints_003E5__41));
											_003CbaseWidth1_003E5__42 = _003C_003E4__this.GetRoadWidthAtStation(project, _003CstartStation_003E5__39);
											_003CbaseWidth3_003E5__43 = _003C_003E4__this.GetRoadWidthAtStation(project, _003CendStation_003E5__40);
											_003Cw1_003E5__44 = _003C_003E4__this.GetLayerRoadWidth(_003CbaseWidth1_003E5__42, _003CbaseWidth_003E5__26, _003C_003E8__22.layer);
											_003Cw3_003E5__45 = _003C_003E4__this.GetLayerRoadWidth(_003CbaseWidth3_003E5__43, _003CbaseWidth_003E5__26, _003C_003E8__22.layer);
											_003Cw2_003E5__46 = (_003Cw1_003E5__44 + _003Cw3_003E5__45) / 2.0;
											_003CbatchWidths_003E5__28.Add((_003Cw1_003E5__44, _003Cw2_003E5__46, _003Cw3_003E5__45));
											_003CmidStation_003E5__47 = (_003CstartStation_003E5__39 + _003CendStation_003E5__40) / 2.0;
											_003Ca1_003E5__48 = _003C_003E4__this.GetTangentClockwiseAngle(project, _003CstartStation_003E5__39);
											_003Ca2_003E5__49 = _003C_003E4__this.GetTangentClockwiseAngle(project, _003CmidStation_003E5__47);
											_003Ca3_003E5__50 = _003C_003E4__this.GetTangentClockwiseAngle(project, _003CendStation_003E5__40);
											_003CoffsetAngle1_003E5__51 = _003C_003E4__this.GetCrossSectionOffsetAngleAtStation(project, _003CstartStation_003E5__39);
											_003CoffsetAngle2_003E5__52 = _003C_003E4__this.GetCrossSectionOffsetAngleAtStation(project, _003CmidStation_003E5__47);
											_003CoffsetAngle3_003E5__53 = _003C_003E4__this.GetCrossSectionOffsetAngleAtStation(project, _003CendStation_003E5__40);
											_003CbatchAngles_003E5__29.Add((_003Ca1_003E5__48, _003Ca2_003E5__49, _003Ca3_003E5__50));
											_003CbatchOffsetAngles_003E5__30.Add((_003CoffsetAngle1_003E5__51, _003CoffsetAngle2_003E5__52, _003CoffsetAngle3_003E5__53));
											_003Cpoints_003E5__41 = null;
										}
										_003Ci_003E5__38++;
									}
									if (_003CbatchData_003E5__27.Count > 0)
									{
										_003CcreatedIds_003E5__54 = ((ItemFactoryBase)_003Cdoc_003E5__1.Create).NewFamilyInstances2(_003CbatchData_003E5__27);
										_003CbatchIndex_003E5__55 = 0;
										_003C_003Es__56 = _003CcreatedIds_003E5__54.GetEnumerator();
										try
										{
											while (_003C_003Es__56.MoveNext())
											{
												_003Cid_003E5__57 = _003C_003Es__56.Current;
												Element element = _003Cdoc_003E5__1.GetElement(_003Cid_003E5__57);
												_003Cinstance_003E5__58 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
												if (_003Cinstance_003E5__58 != null)
												{
													_003C_003E4__this.SetInstanceParameter(_003Cinstance_003E5__58, "O_顶部向上偏移", _003CtopOffsetFeet_003E5__25 * 304.8);
													(_003Cw1_003E5__59, _003Cw2_003E5__60, _003Cw3_003E5__61) = _003CbatchWidths_003E5__28[_003CbatchIndex_003E5__55];
													(_003Ca1_003E5__62, _003Ca2_003E5__63, _003Ca3_003E5__64) = _003CbatchAngles_003E5__29[_003CbatchIndex_003E5__55];
													(_003Coa1_003E5__65, _003Coa2_003E5__66, _003Coa3_003E5__67) = _003CbatchOffsetAngles_003E5__30[_003CbatchIndex_003E5__55];
													_003C_003E4__this.SetInstanceParameter(_003Cinstance_003E5__58, "O_向左侧偏移距离_1", 0.0);
													_003C_003E4__this.SetInstanceParameter(_003Cinstance_003E5__58, "O_向左侧偏移距离_2", 0.0);
													_003C_003E4__this.SetInstanceParameter(_003Cinstance_003E5__58, "O_向左侧偏移距离_3", 0.0);
													_003C_003E4__this.SetInstanceParameter(_003Cinstance_003E5__58, "W_路面宽_1", _003Cw1_003E5__59 * 1000.0);
													_003C_003E4__this.SetInstanceParameter(_003Cinstance_003E5__58, "W_路面宽_2", _003Cw2_003E5__60 * 1000.0);
													_003C_003E4__this.SetInstanceParameter(_003Cinstance_003E5__58, "W_路面宽_3", _003Cw3_003E5__61 * 1000.0);
													_003C_003E4__this.SetAngleParameter(_003Cinstance_003E5__58, "P_切面角度_1", _003Ca1_003E5__62);
													_003C_003E4__this.SetAngleParameter(_003Cinstance_003E5__58, "P_切面角度_2", _003Ca2_003E5__63);
													_003C_003E4__this.SetAngleParameter(_003Cinstance_003E5__58, "P_切面角度_3", _003Ca3_003E5__64);
													_003C_003E4__this.SetAngleParameter(_003Cinstance_003E5__58, "P_偏角_1", _003Coa1_003E5__65);
													_003C_003E4__this.SetAngleParameter(_003Cinstance_003E5__58, "P_偏角_2", _003Coa2_003E5__66);
													_003C_003E4__this.SetAngleParameter(_003Cinstance_003E5__58, "P_偏角_3", _003Coa3_003E5__67);
													_003ClayerInstanceCounts_003E5__4[_003C_003E8__22.layer.Name]++;
													_003Cresult_003E5__3.CreatedInstanceIds.Add(_003Cid_003E5__57.Value.ToString());
													_003CbatchIndex_003E5__55++;
												}
												_003Cinstance_003E5__58 = null;
												_003Cid_003E5__57 = null;
											}
										}
										finally
										{
											if (num < 0 && _003C_003Es__56 != null)
											{
												_003C_003Es__56.Dispose();
											}
										}
										_003C_003Es__56 = null;
										RoadModelingResult obj3 = _003Cresult_003E5__3;
										obj3.CreatedInstanceCount += _003CcreatedIds_003E5__54.Count;
										_003CcreatedIds_003E5__54 = null;
									}
									_003CbatchData_003E5__27 = null;
									_003CbatchWidths_003E5__28 = null;
									_003CbatchAngles_003E5__29 = null;
									_003CbatchOffsetAngles_003E5__30 = null;
									_003C_003E8__22 = null;
									_003Csymbol_003E5__23 = null;
								}
							}
							finally
							{
								if (num < 0 && _003C_003Es__21 != null)
								{
									_003C_003Es__21.Dispose();
								}
							}
							_003C_003Es__21 = null;
							_003Cresult_003E5__3.LayerInstanceCounts = _003ClayerInstanceCounts_003E5__4;
							_003Cresult_003E5__3.IsSuccess = true;
							RoadModelingResult obj4 = _003Cresult_003E5__3;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(18, 2);
							defaultInterpolatedStringHandler5.AppendLiteral("成功创建 ");
							defaultInterpolatedStringHandler5.AppendFormatted(_003Cresult_003E5__3.CreatedInstanceCount);
							defaultInterpolatedStringHandler5.AppendLiteral(" 个路基路面实例，共 ");
							defaultInterpolatedStringHandler5.AppendFormatted(_003CfamilyTypes_003E5__15.Count);
							defaultInterpolatedStringHandler5.AppendLiteral(" 层");
							obj4.Message = defaultInterpolatedStringHandler5.ToStringAndClear();
							_003Ctransaction_003E5__14.Commit();
							_003C_003E4__this._logger.Info("[RoadModelingService] 建模完成: " + _003Cresult_003E5__3.Message);
							result = Result<RoadModelingResult>.Success(_003Cresult_003E5__3);
						}
						catch (Exception ex)
						{
							_003Cex_003E5__68 = ex;
							_003Ctransaction_003E5__14.RollBack();
							_003C_003E4__this._logger.Error("[RoadModelingService] 创建实例失败", _003Cex_003E5__68);
							_003Cresult_003E5__3.Error = _003Cex_003E5__68.Message;
							result = Result<RoadModelingResult>.Failure("创建实例失败: " + _003Cex_003E5__68.Message);
						}
					}
					finally
					{
						if (num < 0 && _003Ctransaction_003E5__14 != null)
						{
							((IDisposable)_003Ctransaction_003E5__14).Dispose();
						}
					}
				}
				else
				{
					result = Result<RoadModelingResult>.Failure("无法找到或加载族: " + _003CtargetFamilyName_003E5__5);
				}
				end_IL_000b:;
			}
			catch (Exception ex)
			{
				_003Cex_003E5__69 = ex;
				_003C_003E4__this._logger.Error("[RoadModelingService] 创建路基路面模型失败", _003Cex_003E5__69);
				result = Result<RoadModelingResult>.Failure("创建失败: " + _003Cex_003E5__69.Message);
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[CompilerGenerated]
	public sealed class _003CFindOrLoadFamilyAsync_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<Family> _003C_003Et__builder;

		public Document doc;

		public string familyName;

		public RoadModelingService _003C_003E4__this;

		private _003C_003Ec__DisplayClass4_0 _003C_003E8__1;

		private FilteredElementCollector _003Ccollector_003E5__2;

		private Family _003CexistingFamily_003E5__3;

		private void MoveNext()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			_003C_003E8__1 = new _003C_003Ec__DisplayClass4_0();
			_003C_003E8__1.familyName = familyName;
			_003Ccollector_003E5__2 = new FilteredElementCollector(doc);
			_003CexistingFamily_003E5__3 = ((IEnumerable)_003Ccollector_003E5__2.OfClass(typeof(Family))).Cast<Family>().FirstOrDefault((Family f) => ((Element)f).Name.Equals(_003C_003E8__1.familyName, StringComparison.OrdinalIgnoreCase));
			Family result;
			if (_003CexistingFamily_003E5__3 != null)
			{
				_003C_003E4__this._logger.Info("[RoadModelingService] 族已存在于项目中: " + _003C_003E8__1.familyName);
				result = _003CexistingFamily_003E5__3;
			}
			else
			{
				_003C_003E4__this._logger.Error("[RoadModelingService] 无法找到族: " + _003C_003E8__1.familyName + "（请确认族文件已上传到族库并被 Handler 加载）", (Exception)null);
				result = null;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003Ccollector_003E5__2 = null;
			_003CexistingFamily_003E5__3 = null;
			_003C_003Et__builder.SetResult(result);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	private readonly ILogger _logger;

	private readonly InfrastructureService _infrastructureService;

	public RoadModelingService()
	{
		_logger = ServiceProvider.GetLogger();
		_infrastructureService = new InfrastructureService();
	}

	[AsyncStateMachine(typeof(_003CCreateRoadModelAsync_003Ed__3))]
	[DebuggerStepThrough]
	public Task<Result<RoadModelingResult>> CreateRoadModelAsync(object document, RoadProject project, bool splitAtIntegerStations = true, int integerStationInterval = 20)
	{
		_003CCreateRoadModelAsync_003Ed__3 stateMachine = new _003CCreateRoadModelAsync_003Ed__3();
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<Result<RoadModelingResult>>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.document = document;
		stateMachine.project = project;
		stateMachine.splitAtIntegerStations = splitAtIntegerStations;
		stateMachine.integerStationInterval = integerStationInterval;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(_003CFindOrLoadFamilyAsync_003Ed__4))]
	private Task<Family?> FindOrLoadFamilyAsync(Document doc, string familyName)
	{
		_003CFindOrLoadFamilyAsync_003Ed__4 stateMachine = new _003CFindOrLoadFamilyAsync_003Ed__4();
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<Family>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.doc = doc;
		stateMachine.familyName = familyName;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private FamilySymbol? GetOrCreateFamilyType(Document doc, Family family, string typeName, SubgradeLayerData layer, List<SubgradeLayerData> allLayers)
	{
		foreach (ElementId familySymbolId in family.GetFamilySymbolIds())
		{
			Element element = doc.GetElement(familySymbolId);
			FamilySymbol val = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
			if (val != null && ((Element)val).Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
			{
				_logger.Info("[RoadModelingService] 族类型已存在: " + typeName);
				return val;
			}
		}
		ElementId val2 = family.GetFamilySymbolIds().FirstOrDefault();
		if (val2 == (ElementId)null)
		{
			_logger.Error("[RoadModelingService] 族没有任何类型", (Exception)null);
			return null;
		}
		Element element2 = doc.GetElement(val2);
		FamilySymbol val3 = (FamilySymbol)(object)((element2 is FamilySymbol) ? element2 : null);
		if (val3 == null)
		{
			return null;
		}
		try
		{
			ElementType obj = ((ElementType)val3).Duplicate(typeName);
			FamilySymbol val4 = (FamilySymbol)(object)((obj is FamilySymbol) ? obj : null);
			if (val4 == null)
			{
				_logger.Error("[RoadModelingService] 复制族类型失败: " + typeName, (Exception)null);
				return null;
			}
			SetTypeParameter(val4, "T_厚度", layer.Height);
			SetTypeParameterAngle(val4, "D_侧面放坡角度", layer.SlopeRatio);
			SetTypeParameter(val4, "S_底部坡度_%", 0.0);
			SetTypeParameter(val4, "S_顶部坡度_%", 0.0);
			int count = allLayers.Count;
			int currentIndex = allLayers.IndexOf(layer);
			Element orCreateMaterial = GetOrCreateMaterial(doc, layer.Name, count, currentIndex);
			if (orCreateMaterial != null)
			{
				SetTypeParameterMaterial(val4, "M_材质", orCreateMaterial.Id);
			}
			_logger.Info("[RoadModelingService] 创建族类型: " + typeName);
			return val4;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 创建族类型失败: " + typeName, ex);
			return null;
		}
	}

	private Element? GetOrCreateMaterial(Document doc, string materialName, int totalLayers, int currentIndex)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		FilteredElementCollector val = new FilteredElementCollector(doc);
		Material val2 = ((IEnumerable)val.OfClass(typeof(Material))).Cast<Material>().FirstOrDefault((Material m) => ((Element)m).Name.Equals(materialName, StringComparison.OrdinalIgnoreCase));
		if (val2 != null)
		{
			return (Element?)(object)val2;
		}
		try
		{
			ElementId val3 = Material.Create(doc, materialName);
			Element element = doc.GetElement(val3);
			Material val4 = (Material)(object)((element is Material) ? element : null);
			if (val4 != null)
			{
				double num = ((totalLayers > 1) ? ((double)currentIndex / (double)(totalLayers - 1)) : 0.0);
				byte b = (byte)(229.0 * num);
				val4.Color = new Color(b, b, b);
				ILogger logger = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 4);
				defaultInterpolatedStringHandler.AppendLiteral("[RoadModelingService] 创建新材质: ");
				defaultInterpolatedStringHandler.AppendFormatted(materialName);
				defaultInterpolatedStringHandler.AppendLiteral(", 颜色 RGB(");
				defaultInterpolatedStringHandler.AppendFormatted(b);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(b);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(b);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return (Element?)(object)val4;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 创建材质失败: " + materialName, ex);
			return null;
		}
	}

	private void SetTypeParameter(FamilySymbol symbol, string paramName, double valueMm)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)symbol).LookupParameter(paramName);
			if (val == null)
			{
				_logger.Warning("[RoadModelingService] 未找到类型参数: " + paramName);
				return;
			}
			if ((int)val.StorageType != 2)
			{
				_logger.Warning("[RoadModelingService] 参数 " + paramName + " 不是双精度类型");
				return;
			}
			double num = UnitUtils.ConvertToInternalUnits(valueMm, UnitTypeId.Millimeters);
			val.Set(num);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 设置类型参数失败: " + paramName, ex);
		}
	}

	private void SetTypeParameterAngle(FamilySymbol symbol, string paramName, double slopeRatio)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)symbol).LookupParameter(paramName);
			if (val == null)
			{
				_logger.Warning("[RoadModelingService] 未找到角度类型参数: " + paramName);
				return;
			}
			if ((int)val.StorageType != 2)
			{
				_logger.Warning("[RoadModelingService] 角度参数 " + paramName + " 不是双精度类型");
				return;
			}
			double num = ((!(slopeRatio <= 0.0)) ? Math.Atan(1.0 / slopeRatio) : (Math.PI / 2.0));
			val.Set(num);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 设置角度类型参数失败: " + paramName, ex);
		}
	}

	private void SetTypeParameterMaterial(FamilySymbol symbol, string paramName, ElementId materialId)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)symbol).LookupParameter(paramName);
			if (val == null)
			{
				_logger.Warning("[RoadModelingService] 未找到类型参数: " + paramName);
			}
			else if ((int)val.StorageType != 4)
			{
				_logger.Warning("[RoadModelingService] 参数 " + paramName + " 不是 ElementId 类型");
			}
			else
			{
				val.Set(materialId);
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 设置类型参数失败: " + paramName, ex);
		}
	}

	private void SetInstanceParameter(FamilyInstance instance, string paramName, double valueMm)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)instance).LookupParameter(paramName);
			if (val == null)
			{
				_logger.Warning("[RoadModelingService] 未找到实例参数: " + paramName);
				return;
			}
			if ((int)val.StorageType != 2)
			{
				_logger.Warning("[RoadModelingService] 参数 " + paramName + " 不是双精度类型");
				return;
			}
			double num = UnitUtils.ConvertToInternalUnits(valueMm, UnitTypeId.Millimeters);
			val.Set(num);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 设置实例参数失败: " + paramName, ex);
		}
	}

	private void SetIntegerParameter(FamilyInstance instance, string paramName, int value)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)instance).LookupParameter(paramName);
			if (val == null)
			{
				_logger.Warning("[RoadModelingService] 未找到整数参数: " + paramName);
			}
			else if ((int)val.StorageType == 1)
			{
				val.Set(value);
			}
			else if ((int)val.StorageType == 2)
			{
				val.Set((double)value);
			}
			else
			{
				_logger.Warning("[RoadModelingService] 参数 " + paramName + " 不是整数或双精度类型");
			}
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 设置整数参数失败: " + paramName, ex);
		}
	}

	private void SetAngleParameter(FamilyInstance instance, string paramName, double angleDeg)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		try
		{
			Parameter val = ((Element)instance).LookupParameter(paramName);
			if (val == null)
			{
				_logger.Warning("[RoadModelingService] 未找到角度参数: " + paramName);
				return;
			}
			if ((int)val.StorageType != 2)
			{
				_logger.Warning("[RoadModelingService] 角度参数 " + paramName + " 不是双精度类型");
				return;
			}
			double num = angleDeg * Math.PI / 180.0;
			val.Set(num);
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 设置角度参数失败: " + paramName, ex);
		}
	}

	private List<double> CalculateStationPositions(RoadProject project, bool splitAtIntegerStations, int integerStationInterval)
	{
		List<double> list = new List<double>();
		if (project.Centerline3DPoints == null || project.Centerline3DPoints.Count == 0)
		{
			return list;
		}
		double startStationKm = project.Centerline3DPoints[0].StationKm;
		List<RoadCenterlinePoint3D> centerline3DPoints = project.Centerline3DPoints;
		double endStationKm = centerline3DPoints[centerline3DPoints.Count - 1].StationKm;
		double num = startStationKm * 1000.0;
		double num2 = endStationKm * 1000.0;
		StationParametersConfiguration stationParametersConfiguration = project.StationParametersConfiguration;
		List<StationParameterData> list2 = ((stationParametersConfiguration != null) ? stationParametersConfiguration.Parameters : null);
		if (list2 != null && list2.Count > 0)
		{
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[RoadModelingService] 使用平面布置中定义的桩号位置，共 ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			list = (from p in list2
				select p.StationKm into s
				orderby s
				select s).ToList();
			if (list[0] > startStationKm)
			{
				list.Insert(0, startStationKm);
			}
			if (list[list.Count - 1] < endStationKm)
			{
				list.Add(endStationKm);
			}
			list = list.Where((double s) => s >= startStationKm && s <= endStationKm).Distinct().ToList();
			_logger.Info("[RoadModelingService] 最终桩号位置: " + string.Join(", ", list.Select(delegate(double s)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("K");
				defaultInterpolatedStringHandler3.AppendFormatted(s * 1000.0, "F2");
				return defaultInterpolatedStringHandler3.ToStringAndClear();
			})));
			return list;
		}
		ILogger logger2 = _logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(45, 1);
		defaultInterpolatedStringHandler2.AppendLiteral("[RoadModelingService] 未定义桩号位置，使用整数间隔切分 (间隔=");
		defaultInterpolatedStringHandler2.AppendFormatted(integerStationInterval);
		defaultInterpolatedStringHandler2.AppendLiteral("m)");
		logger2.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		if (splitAtIntegerStations)
		{
			for (double num3 = num; num3 <= num2; num3 += (double)integerStationInterval)
			{
				list.Add(num3 / 1000.0);
			}
			if (list[list.Count - 1] < endStationKm)
			{
				list.Add(endStationKm);
			}
		}
		else
		{
			list.AddRange(project.Centerline3DPoints.Select((RoadCenterlinePoint3D p) => p.StationKm));
		}
		list = (from s in list.Distinct()
			orderby s
			select s).ToList();
		_logger.Info("[RoadModelingService] 桩号位置: " + string.Join(", ", list.Select(delegate(double s)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(1, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("K");
			defaultInterpolatedStringHandler3.AppendFormatted(s * 1000.0, "F2");
			return defaultInterpolatedStringHandler3.ToStringAndClear();
		})));
		return list;
	}

	private IList<XYZ>? CalculateAdaptivePoints(RoadProject project, double startStationKm, double endStationKm)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Invalid comparison between Unknown and I4
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Expected O, but got Unknown
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Expected O, but got Unknown
		try
		{
			double item = project.GlobalOriginOffset.Item1;
			double item2 = project.GlobalOriginOffset.Item2;
			double item3 = project.GlobalOriginOffset.Item3;
			RoadCenterlinePoint3D val = null;
			RoadCenterlinePoint3D val2 = null;
			RoadCenterlinePoint3D val3 = null;
			if ((int)project.DataSource == 2 && project.HorizontalCurveTable != null && project.VerticalCurveTable != null)
			{
				Result<RoadCenterlinePoint3D> val4 = _infrastructureService.GeneratePointAtStation(project.HorizontalCurveTable, project.VerticalCurveTable, startStationKm);
				if (val4.IsSuccess && val4.Value != null)
				{
					val = val4.Value;
				}
				double stationKm = (startStationKm + endStationKm) / 2.0;
				Result<RoadCenterlinePoint3D> val5 = _infrastructureService.GeneratePointAtStation(project.HorizontalCurveTable, project.VerticalCurveTable, stationKm);
				if (val5.IsSuccess && val5.Value != null)
				{
					val2 = val5.Value;
				}
				Result<RoadCenterlinePoint3D> val6 = _infrastructureService.GeneratePointAtStation(project.HorizontalCurveTable, project.VerticalCurveTable, endStationKm);
				if (val6.IsSuccess && val6.Value != null)
				{
					val3 = val6.Value;
				}
			}
			if (val == null)
			{
				val = project.GetPointAtStation(startStationKm);
			}
			if (val2 == null)
			{
				val2 = project.GetPointAtStation((startStationKm + endStationKm) / 2.0);
			}
			if (val3 == null)
			{
				val3 = project.GetPointAtStation(endStationKm);
			}
			if (val == null || val3 == null)
			{
				ILogger logger = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 4);
				defaultInterpolatedStringHandler.AppendLiteral("[RoadModelingService] 计算自适应点失败: 起点=");
				defaultInterpolatedStringHandler.AppendFormatted(startStationKm, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(", 终点=");
				defaultInterpolatedStringHandler.AppendFormatted(endStationKm, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendLiteral("起点状态=");
				defaultInterpolatedStringHandler.AppendFormatted(val != null);
				defaultInterpolatedStringHandler.AppendLiteral(", 终点状态=");
				defaultInterpolatedStringHandler.AppendFormatted(val3 != null);
				logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (val2 == null)
			{
				val2 = new RoadCenterlinePoint3D
				{
					StationKm = (startStationKm + endStationKm) / 2.0,
					X = (val.X + val3.X) / 2.0,
					Y = (val.Y + val3.Y) / 2.0,
					Z = (val.Z + val3.Z) / 2.0,
					Azimuth = (val.Azimuth + val3.Azimuth) / 2.0
				};
			}
			return new List<XYZ>
			{
				new XYZ((val.X - item) / 0.3048, (val.Y - item2) / 0.3048, (val.Z - item3) / 0.3048),
				new XYZ((val2.X - item) / 0.3048, (val2.Y - item2) / 0.3048, (val2.Z - item3) / 0.3048),
				new XYZ((val3.X - item) / 0.3048, (val3.Y - item2) / 0.3048, (val3.Z - item3) / 0.3048)
			};
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 计算自适应点失败", ex);
			return null;
		}
	}

	private double GetRoadWidthAtStation(RoadProject project, double stationKm)
	{
		StationParametersConfiguration stationParametersConfiguration = project.StationParametersConfiguration;
		double num = ((stationParametersConfiguration != null) ? stationParametersConfiguration.DefaultRoadWidthM : 7.0);
		StationParametersConfiguration stationParametersConfiguration2 = project.StationParametersConfiguration;
		List<StationParameterData> list = ((stationParametersConfiguration2 != null) ? stationParametersConfiguration2.Parameters : null);
		if (list == null || list.Count == 0)
		{
			return num;
		}
		List<StationParameterData> list2 = list.OrderBy((StationParameterData p) => p.StationKm).ToList();
		StationParameterData val = null;
		StationParameterData val2 = null;
		foreach (StationParameterData item in list2)
		{
			if (item.StationKm <= stationKm)
			{
				val = item;
			}
			if (item.StationKm >= stationKm && val2 == null)
			{
				val2 = item;
			}
		}
		if (val == null && val2 == null)
		{
			return num;
		}
		if (val == null)
		{
			return val2.RoadWidth ?? num;
		}
		if (val2 == null)
		{
			return val.RoadWidth ?? num;
		}
		double num2 = val.RoadWidth ?? num;
		double num3 = val2.RoadWidth ?? num;
		if (Math.Abs(val2.StationKm - val.StationKm) < 0.0001)
		{
			return num2;
		}
		double num4 = (stationKm - val.StationKm) / (val2.StationKm - val.StationKm);
		return num2 + (num3 - num2) * num4;
	}

	private double GetCrossSectionOffsetAngleAtStation(RoadProject project, double stationKm)
	{
		double num = 0.0;
		StationParametersConfiguration stationParametersConfiguration = project.StationParametersConfiguration;
		List<StationParameterData> list = ((stationParametersConfiguration != null) ? stationParametersConfiguration.Parameters : null);
		if (list == null || list.Count == 0)
		{
			return num;
		}
		List<StationParameterData> list2 = list.OrderBy((StationParameterData p) => p.StationKm).ToList();
		StationParameterData val = null;
		StationParameterData val2 = null;
		foreach (StationParameterData item in list2)
		{
			if (item.StationKm <= stationKm)
			{
				val = item;
			}
			if (item.StationKm >= stationKm && val2 == null)
			{
				val2 = item;
			}
		}
		if (val == null && val2 == null)
		{
			return num;
		}
		if (val == null)
		{
			return val2.CrossSectionOffsetAngle ?? num;
		}
		if (val2 == null)
		{
			return val.CrossSectionOffsetAngle ?? num;
		}
		double num2 = val.CrossSectionOffsetAngle ?? num;
		double num3 = val2.CrossSectionOffsetAngle ?? num;
		if (Math.Abs(val2.StationKm - val.StationKm) < 0.0001)
		{
			return num2;
		}
		double num4 = (stationKm - val.StationKm) / (val2.StationKm - val.StationKm);
		return num2 + (num3 - num2) * num4;
	}

	private double GetLayerRoadWidth(double baseRoadWidth, double cumulativeBaseWidth, SubgradeLayerData layer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		double num = (((int)layer.IncrementType == 0) ? (layer.WidthIncrement * 2.0) : layer.WidthIncrement);
		double num2 = num / 1000.0;
		return baseRoadWidth + cumulativeBaseWidth + num2;
	}

	private double GetTangentClockwiseAngle(RoadProject project, double stationKm)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		double azimuth;
		if ((int)project.DataSource == 2 && project.HorizontalCurveTable != null && project.VerticalCurveTable != null)
		{
			Result<RoadCenterlinePoint3D> val = _infrastructureService.GeneratePointAtStation(project.HorizontalCurveTable, project.VerticalCurveTable, stationKm);
			if (val.IsSuccess && val.Value != null)
			{
				azimuth = val.Value.Azimuth;
				return azimuth * 180.0 / Math.PI;
			}
		}
		RoadCenterlinePoint3D pointAtStation = project.GetPointAtStation(stationKm);
		if (pointAtStation == null)
		{
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[RoadModelingService] 桩号 K");
			defaultInterpolatedStringHandler.AppendFormatted(stationKm * 1000.0, "F1");
			defaultInterpolatedStringHandler.AppendLiteral(": 无法获取点位");
			logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return 0.0;
		}
		azimuth = pointAtStation.Azimuth;
		return azimuth * 180.0 / Math.PI;
	}

	private static string GetFamilyNameByStructureType(AncillaryStructureType structureType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected I4, but got Unknown
		return (int)structureType switch
		{
			0 => "AST_R_直角梯形_3", 
			1 => "AST_R_矩形_3", 
			2 => "AST_R_矩形空心_3", 
			3 => "AST_R_单坡面层_3", 
			4 => "AST_R_双坡面层_3", 
			5 => "AST_R_圆角路沿石_3", 
			_ => "AST_R_矩形_3", 
		};
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(_003CCreateAncillaryStructureAsync_003Ed__20))]
	public Task<Result<RoadModelingResult>> CreateAncillaryStructureAsync(object document, RoadProject project)
	{
		_003CCreateAncillaryStructureAsync_003Ed__20 stateMachine = new _003CCreateAncillaryStructureAsync_003Ed__20();
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<Result<RoadModelingResult>>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.document = document;
		stateMachine.project = project;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private FamilySymbol? GetOrCreateFamilyTypeForAncillaryStructure(Document doc, Family family, string typeName, AncillaryStructureData structure)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Invalid comparison between Unknown and I4
		foreach (ElementId familySymbolId in family.GetFamilySymbolIds())
		{
			Element element = doc.GetElement(familySymbolId);
			FamilySymbol val = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
			if (val != null && ((Element)val).Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
			{
				return val;
			}
		}
		ElementId val2 = family.GetFamilySymbolIds().FirstOrDefault();
		if (val2 == (ElementId)null)
		{
			_logger.Error("[RoadModelingService] 族没有任何类型: " + ((Element)family).Name, (Exception)null);
			return null;
		}
		Element element2 = doc.GetElement(val2);
		FamilySymbol val3 = (FamilySymbol)(object)((element2 is FamilySymbol) ? element2 : null);
		if (val3 == null)
		{
			return null;
		}
		try
		{
			ElementType obj = ((ElementType)val3).Duplicate(typeName);
			FamilySymbol val4 = (FamilySymbol)(object)((obj is FamilySymbol) ? obj : null);
			if (val4 == null)
			{
				_logger.Error("[RoadModelingService] 复制族类型失败: " + typeName, (Exception)null);
				return null;
			}
			SetTypeParameter(val4, "H_高度", structure.Height);
			SetTypeParameter(val4, "W_宽度", structure.Width);
			if ((int)structure.StructureType == 0 && structure.SlopeRatio.HasValue && structure.SlopeRatio.Value > 0.0)
			{
				SetTypeParameterAngle(val4, "D_侧面放坡角度", structure.SlopeRatio.Value);
			}
			Element orCreateMaterialForAncillaryStructure = GetOrCreateMaterialForAncillaryStructure(doc, structure.Name, structure.Color);
			if (orCreateMaterialForAncillaryStructure != null)
			{
				SetTypeParameterMaterial(val4, "M_材质", orCreateMaterialForAncillaryStructure.Id);
			}
			_logger.Info("[RoadModelingService] 创建附属结构族类型: " + typeName);
			return val4;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 创建族类型失败: " + typeName, ex);
			return null;
		}
	}

	private Element? GetOrCreateMaterialForAncillaryStructure(Document doc, string materialName, string colorHex)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		Material val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Material))).Cast<Material>().FirstOrDefault((Material m) => ((Element)m).Name.Equals(materialName, StringComparison.OrdinalIgnoreCase));
		if (val != null)
		{
			return (Element?)(object)val;
		}
		try
		{
			ElementId val2 = Material.Create(doc, materialName);
			Element element = doc.GetElement(val2);
			Material val3 = (Material)(object)((element is Material) ? element : null);
			if (val3 != null)
			{
				if (!string.IsNullOrEmpty(colorHex) && colorHex.StartsWith("#") && colorHex.Length == 7)
				{
					byte b = byte.Parse(colorHex.Substring(1, 2), NumberStyles.HexNumber);
					byte b2 = byte.Parse(colorHex.Substring(3, 2), NumberStyles.HexNumber);
					byte b3 = byte.Parse(colorHex.Substring(5, 2), NumberStyles.HexNumber);
					val3.Color = new Color(b, b2, b3);
				}
				_logger.Info("[RoadModelingService] 创建附属结构材质: " + materialName + ", 颜色 " + colorHex);
			}
			return (Element?)(object)val3;
		}
		catch (Exception ex)
		{
			_logger.Error("[RoadModelingService] 创建材质失败: " + materialName, ex);
			return null;
		}
	}

	private List<(double StartKm, double EndKm)> SplitLayoutSegment(AncillaryStructureLayoutSegmentData segment)
	{
		List<(double, double)> list = new List<(double, double)>();
		double num = ((segment.ModelLength > 0.0) ? (segment.ModelLength / 1000.0) : 0.005);
		double num2 = segment.StartStationKm;
		while (num2 < segment.EndStationKm - 1E-09)
		{
			double num3 = Math.Min(num2 + num, segment.EndStationKm);
			list.Add((num2, num3));
			num2 = num3;
		}
		return list;
	}

	private List<string> CreateInstancesForSubSegments(Document doc, RoadProject project, AncillaryStructureData structure, AncillaryPositionType positionType, List<(double StartKm, double EndKm)> subSegments, FamilySymbol symbol)
	{
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Invalid comparison between Unknown and I4
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Invalid comparison between Unknown and I4
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Invalid comparison between Unknown and I4
		List<string> list = new List<string>();
		List<FamilyInstanceCreationData> list2 = new List<FamilyInstanceCreationData>();
		List<(double, double, double, double, double, double, double, double, double, XYZ, XYZ, XYZ)> list3 = new List<(double, double, double, double, double, double, double, double, double, XYZ, XYZ, XYZ)>();
		foreach (var subSegment in subSegments)
		{
			double item = subSegment.StartKm;
			double item2 = subSegment.EndKm;
			IList<XYZ> list4 = CalculateAdaptivePoints(project, item, item2);
			if (list4 != null && list4.Count == 3)
			{
				list2.Add(new FamilyInstanceCreationData(symbol, list4));
				double stationKm = (item + item2) / 2.0;
				double roadWidthAtStation = GetRoadWidthAtStation(project, item);
				double roadWidthAtStation2 = GetRoadWidthAtStation(project, stationKm);
				double roadWidthAtStation3 = GetRoadWidthAtStation(project, item2);
				double item3 = roadWidthAtStation / 2.0 * 1000.0 + structure.HorizontalOffset;
				double item4 = roadWidthAtStation2 / 2.0 * 1000.0 + structure.HorizontalOffset;
				double item5 = roadWidthAtStation3 / 2.0 * 1000.0 + structure.HorizontalOffset;
				double tangentClockwiseAngle = GetTangentClockwiseAngle(project, item);
				double tangentClockwiseAngle2 = GetTangentClockwiseAngle(project, stationKm);
				double tangentClockwiseAngle3 = GetTangentClockwiseAngle(project, item2);
				double crossSectionOffsetAngleAtStation = GetCrossSectionOffsetAngleAtStation(project, item);
				double crossSectionOffsetAngleAtStation2 = GetCrossSectionOffsetAngleAtStation(project, stationKm);
				double crossSectionOffsetAngleAtStation3 = GetCrossSectionOffsetAngleAtStation(project, item2);
				list3.Add((item3, item4, item5, tangentClockwiseAngle, tangentClockwiseAngle2, tangentClockwiseAngle3, crossSectionOffsetAngleAtStation, crossSectionOffsetAngleAtStation2, crossSectionOffsetAngleAtStation3, list4[0], list4[1], list4[2]));
			}
		}
		if (list2.Count == 0)
		{
			return list;
		}
		ICollection<ElementId> collection = ((ItemFactoryBase)doc.Create).NewFamilyInstances2(list2);
		List<ElementId> list5 = new List<ElementId>(collection);
		for (int i = 0; i < list5.Count; i++)
		{
			Element element = doc.GetElement(list5[i]);
			FamilyInstance val = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			if (val == null)
			{
				continue;
			}
			(double, double, double, double, double, double, double, double, double, XYZ, XYZ, XYZ) tuple = list3[i];
			try
			{
				SetInstanceParameter(val, "O_水平偏移距离_1", tuple.Item1);
				SetInstanceParameter(val, "O_水平偏移距离_2", tuple.Item2);
				SetInstanceParameter(val, "O_水平偏移距离_3", tuple.Item3);
				SetInstanceParameter(val, "O_顶部向上偏移", structure.ElevationDifference);
				SetAngleParameter(val, "P_切面角度_1", tuple.Item4);
				SetAngleParameter(val, "P_切面角度_2", tuple.Item5);
				SetAngleParameter(val, "P_切面角度_3", tuple.Item6);
				SetAngleParameter(val, "P_偏角_1", tuple.Item7);
				SetAngleParameter(val, "P_偏角_2", tuple.Rest.Item1);
				SetAngleParameter(val, "P_偏角_3", tuple.Rest.Item2);
				bool flag = (int)positionType == 0 || (int)positionType == 2;
				bool flag2 = (int)positionType == 1 || (int)positionType == 2;
				SetIntegerParameter(val, "左", flag ? 1 : 0);
				SetIntegerParameter(val, "右", flag2 ? 1 : 0);
				list.Add(list5[i].Value.ToString());
			}
			catch (Exception ex)
			{
				_logger.Warning("[RoadModelingService] 设置参数失败，删除实例: " + ex.Message);
				try
				{
					doc.Delete(((Element)val).Id);
				}
				catch
				{
				}
			}
		}
		ILogger logger = _logger;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
		defaultInterpolatedStringHandler.AppendLiteral("[RoadModelingService] 附属结构 ");
		defaultInterpolatedStringHandler.AppendFormatted(structure.Name);
		defaultInterpolatedStringHandler.AppendLiteral(" (");
		defaultInterpolatedStringHandler.AppendFormatted<AncillaryPositionType>(positionType);
		defaultInterpolatedStringHandler.AppendLiteral("): 创建 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个实例");
		logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		return list;
	}
}
