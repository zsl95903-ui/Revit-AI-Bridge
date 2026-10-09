using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Units;
using ns2;
using ns6;

namespace RevitAi.Revit.Units;

public sealed class ProjectUnitService : IProjectUnitService
{
	[CompilerGenerated]
	public sealed class Class335 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public object object_0;

		public Dictionary<UnitType, object> dictionary_0;

		public ProjectUnitService projectUnitService_0;

		private Dictionary<UnitType, object>.Enumerator enumerator_0;

		private KeyValuePair<UnitType, object> keyValuePair_0;

		private UnitType unitType_0;

		private object object_1;

		private Exception exception_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			int num = int_0;
			bool result;
			if (object_0 == null)
			{
				Logger.Error("[ProjectUnitService] 批量设置单位失败: 文档为 null");
				result = false;
			}
			else if (dictionary_0 == null || dictionary_0.Count == 0)
			{
				Logger.Warning("[ProjectUnitService] 批量设置单位失败: 无单位设置");
				result = false;
			}
			else
			{
				try
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[ProjectUnitService] 开始批量设置 ");
					defaultInterpolatedStringHandler.AppendFormatted(dictionary_0.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个单位类型");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					enumerator_0 = dictionary_0.GetEnumerator();
					try
					{
						while (enumerator_0.MoveNext())
						{
							keyValuePair_0 = enumerator_0.Current;
							unitType_0 = keyValuePair_0.Key;
							object_1 = keyValuePair_0.Value;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("[ProjectUnitService] 设置单位 ");
							defaultInterpolatedStringHandler2.AppendFormatted<UnitType>(unitType_0);
							defaultInterpolatedStringHandler2.AppendLiteral(" 需要通过 RevitAdapter");
							Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
							object_1 = null;
							keyValuePair_0 = default(KeyValuePair<UnitType, object>);
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
						}
					}
					enumerator_0 = default(Dictionary<UnitType, object>.Enumerator);
					result = true;
				}
				catch (Exception ex)
				{
					exception_0 = ex;
					Logger.Error("[ProjectUnitService] 批量设置单位失败: " + exception_0.Message, exception_0);
					result = false;
				}
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private readonly Dictionary<string, Dictionary<UnitType, ProjectUnitInfo>> dictionary_0 = new Dictionary<string, Dictionary<UnitType, ProjectUnitInfo>>();

	public Dictionary<UnitType, ProjectUnitInfo> GetProjectUnits(object document)
	{
		if (document == null)
		{
			Logger.Error("[ProjectUnitService] 获取项目单位失败: 文档为 null");
			return new Dictionary<UnitType, ProjectUnitInfo>();
		}
		try
		{
			string text = method_0(document);
			if (dictionary_0.ContainsKey(text))
			{
				Logger.Debug("[ProjectUnitService] 从缓存获取单位设置: " + text);
				return dictionary_0[text];
			}
			Dictionary<UnitType, ProjectUnitInfo> dictionary = Class336.smethod_0(document);
			dictionary_0[text] = dictionary;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ProjectUnitService] 成功获取并缓存 ");
			defaultInterpolatedStringHandler.AppendFormatted(dictionary.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个单位类型");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return dictionary;
		}
		catch (Exception ex)
		{
			Logger.Error("[ProjectUnitService] 获取项目单位失败: " + ex.Message, ex);
			return new Dictionary<UnitType, ProjectUnitInfo>();
		}
	}

	public ProjectUnitInfo? GetProjectUnit(object document, UnitType unitType)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Dictionary<UnitType, ProjectUnitInfo> projectUnits = GetProjectUnits(document);
			if (projectUnits.TryGetValue(unitType, out var value))
			{
				return value;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ProjectUnitService] 未找到单位类型 ");
			defaultInterpolatedStringHandler.AppendFormatted<UnitType>(unitType);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			return null;
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[ProjectUnitService] 获取单位 ");
			defaultInterpolatedStringHandler2.AppendFormatted<UnitType>(unitType);
			defaultInterpolatedStringHandler2.AppendLiteral(" 失败: ");
			defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
			Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear(), ex);
			return null;
		}
	}

	public Task<bool> SetProjectUnitAsync(object document, UnitType unitType, object displayUnitType)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (document == null)
		{
			Logger.Error("[ProjectUnitService] 设置项目单位失败: 文档为 null");
			return Task.FromResult(result: false);
		}
		if (displayUnitType == null)
		{
			Logger.Error("[ProjectUnitService] 设置项目单位失败: displayUnitType 为 null");
			return Task.FromResult(result: false);
		}
		try
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ProjectUnitService] 开始设置单位: ");
			defaultInterpolatedStringHandler.AppendFormatted<UnitType>(unitType);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			Logger.Warning("[ProjectUnitService] SetProjectUnitAsync 需要通过 RevitAdapter 调用");
			return Task.FromResult(result: false);
		}
		catch (Exception ex)
		{
			Logger.Error("[ProjectUnitService] 设置单位失败: " + ex.Message, ex);
			return Task.FromResult(result: false);
		}
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class335))]
	public Task<bool> SetProjectUnitsAsync(object document, Dictionary<UnitType, object> unitSettings)
	{
		Class335 stateMachine = new Class335();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.projectUnitService_0 = this;
		stateMachine.object_0 = document;
		stateMachine.dictionary_0 = unitSettings;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public Task<bool> SetMetricUnitsAsync(object document)
	{
		if (document == null)
		{
			Logger.Error("[ProjectUnitService] 设置公制单位失败: 文档为 null");
			return Task.FromResult(result: false);
		}
		try
		{
			Logger.Info("[ProjectUnitService] 开始设置公制单位");
			Dictionary<UnitType, object> unitSettings = new Dictionary<UnitType, object>
			{
				{
					(UnitType)1,
					Class336.smethod_5()
				},
				{
					(UnitType)2,
					Class336.smethod_6()
				},
				{
					(UnitType)3,
					Class336.smethod_7()
				},
				{
					(UnitType)4,
					Class336.smethod_8()
				}
			};
			return SetProjectUnitsAsync(document, unitSettings);
		}
		catch (Exception ex)
		{
			Logger.Error("[ProjectUnitService] 设置公制单位失败: " + ex.Message, ex);
			return Task.FromResult(result: false);
		}
	}

	public Task<bool> SetImperialUnitsAsync(object document)
	{
		if (document == null)
		{
			Logger.Error("[ProjectUnitService] 设置英制单位失败: 文档为 null");
			return Task.FromResult(result: false);
		}
		try
		{
			Logger.Info("[ProjectUnitService] 开始设置英制单位");
			Dictionary<UnitType, object> unitSettings = new Dictionary<UnitType, object>
			{
				{
					(UnitType)1,
					Class336.smethod_9()
				},
				{
					(UnitType)2,
					Class336.smethod_10()
				},
				{
					(UnitType)3,
					Class336.smethod_11()
				},
				{
					(UnitType)4,
					Class336.smethod_8()
				}
			};
			return SetProjectUnitsAsync(document, unitSettings);
		}
		catch (Exception ex)
		{
			Logger.Error("[ProjectUnitService] 设置英制单位失败: " + ex.Message, ex);
			return Task.FromResult(result: false);
		}
	}

	public void CacheProjectUnits(object document)
	{
		if (document == null)
		{
			Logger.Error("[ProjectUnitService] 缓存单位失败: 文档为 null");
			return;
		}
		try
		{
			string key = method_0(document);
			Dictionary<UnitType, ProjectUnitInfo> dictionary = Class336.smethod_0(document);
			dictionary_0[key] = dictionary;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ProjectUnitService] 成功缓存 ");
			defaultInterpolatedStringHandler.AppendFormatted(dictionary.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个单位类型");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Error("[ProjectUnitService] 缓存单位失败: " + ex.Message, ex);
		}
	}

	public void ClearCache(object document)
	{
		if (document == null)
		{
			Logger.Error("[ProjectUnitService] 清除缓存失败: 文档为 null");
			return;
		}
		try
		{
			string text = method_0(document);
			if (dictionary_0.Remove(text))
			{
				Logger.Info("[ProjectUnitService] 清除缓存: " + text);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[ProjectUnitService] 清除缓存失败: " + ex.Message, ex);
		}
	}

	public void ClearAllCache()
	{
		try
		{
			int count = dictionary_0.Count;
			dictionary_0.Clear();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[ProjectUnitService] 清除所有缓存: ");
			defaultInterpolatedStringHandler.AppendFormatted(count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个文档");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Error("[ProjectUnitService] 清除所有缓存失败: " + ex.Message, ex);
		}
	}

	private string method_0(object object_0)
	{
		try
		{
			Type type = object_0.GetType();
			PropertyInfo property = type.GetProperty("DocumentId", BindingFlags.Instance | BindingFlags.Public);
			if (property != null)
			{
				object value = property.GetValue(object_0);
				if (value != null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
					defaultInterpolatedStringHandler.AppendLiteral("doc_");
					defaultInterpolatedStringHandler.AppendFormatted<object>(value);
					return defaultInterpolatedStringHandler.ToStringAndClear();
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("doc_");
			defaultInterpolatedStringHandler2.AppendFormatted(object_0.GetHashCode());
			return defaultInterpolatedStringHandler2.ToStringAndClear();
		}
		catch
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("doc_");
			defaultInterpolatedStringHandler3.AppendFormatted(object_0.GetHashCode());
			return defaultInterpolatedStringHandler3.ToStringAndClear();
		}
	}
}
