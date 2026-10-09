using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using Microsoft.CSharp.RuntimeBinder;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_element_geometry", Category = "几何分析", Description = "获取元素的完整几何信息，包括包围盒、长度、面积、体积、几何点等。返回单位：毫米、平方米、立方米", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetElementGeometryTool : IAITool
{
	[CompilerGenerated]
	private static class Class496
	{
		public static CallSite<Func<CallSite, object, object>> callSite_0;

		public static CallSite<Func<CallSite, object, object>> callSite_1;

		public static CallSite<Func<CallSite, object, object>> callSite_2;

		public static CallSite<Func<CallSite, object, object>> callSite_3;

		public static CallSite<Func<CallSite, object, object>> callSite_4;

		public static CallSite<Func<CallSite, object, object>> callSite_5;

		public static CallSite<Func<CallSite, object, object>> callSite_6;

		public static CallSite<Func<CallSite, object, object>> callSite_7;

		public static CallSite<Func<CallSite, object, object>> callSite_8;

		public static CallSite<Func<CallSite, object, object>> callSite_9;

		public static CallSite<Func<CallSite, object, object>> callSite_10;

		public static CallSite<Func<CallSite, object, object>> callSite_11;

		public static CallSite<Func<CallSite, object, object>> callSite_12;

		public static CallSite<Func<CallSite, object, object>> callSite_13;

		public static CallSite<Func<CallSite, object, object>> callSite_14;

		public static CallSite<Func<CallSite, object, object>> callSite_15;

		public static CallSite<Func<CallSite, object, object>> callSite_16;

		public static CallSite<Func<CallSite, object, object>> callSite_17;

		public static CallSite<Func<CallSite, object, object>> callSite_18;

		public static CallSite<Func<CallSite, object, object>> callSite_19;

		public static CallSite<Func<CallSite, object, object>> callSite_20;

		public static CallSite<Func<CallSite, object, object>> callSite_21;

		public static CallSite<Func<CallSite, object, object>> callSite_22;

		public static CallSite<Func<CallSite, object, object>> callSite_23;

		public static CallSite<Func<CallSite, object, object>> callSite_24;

		public static CallSite<Func<CallSite, object, object>> callSite_25;

		public static CallSite<Func<CallSite, object, object>> callSite_26;

		public static CallSite<Func<CallSite, object, object>> callSite_27;

		public static CallSite<Func<CallSite, object, object>> callSite_28;

		public static CallSite<Func<CallSite, object, object>> callSite_29;

		public static CallSite<Func<CallSite, object, object>> callSite_30;

		public static CallSite<Func<CallSite, object, object>> callSite_31;

		public static CallSite<Func<CallSite, object, object>> callSite_32;

		public static CallSite<Func<CallSite, object, object>> callSite_33;

		public static CallSite<Func<CallSite, Type, string, object, object>> callSite_34;

		public static CallSite<Func<CallSite, object, AIToolResult>> callSite_35;
	}

	[CompilerGenerated]
	private sealed class Class497 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetElementGeometryTool getElementGeometryTool_0;

		private int int_1;

		private string string_0;

		private bool bool_0;

		private IElementService ielementService_0;

		private IGeometryService igeometryService_0;

		private IAnalysisService ianalysisService_0;

		private object object_0;

		private string string_1;

		private string string_2;

		private Class164<int, string, string, string, string, string> class164_0;

		private object object_1;

		private ((double MinX, double MinY, double MinZ)? Min, (double MaxX, double MaxY, double MaxZ)? Max)? nullable_0;

		private Class165<_003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>> class165_0;

		private double? nullable_1;

		private double double_0;

		private double? nullable_2;

		private double? nullable_3;

		private double? nullable_4;

		private double? nullable_5;

		private object object_2;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class497 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter);
				int num = -1;
				int_0 = -1;
			}
			awaiter.GetResult();
			AIToolResult result;
			try
			{
				int_1 = aitoolContext_0.GetParameter<int>("elementId", 0);
				string_0 = aitoolContext_0.GetParameter<string>("areaType", "surface");
				bool_0 = aitoolContext_0.GetParameter<bool>("computeReferences", false);
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter != null) ? revitAdapter.ElementService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				igeometryService_0 = ((revitAdapter2 != null) ? revitAdapter2.GeometryService : null);
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				ianalysisService_0 = ((revitAdapter3 != null) ? revitAdapter3.AnalysisService : null);
				if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (igeometryService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 GeometryService");
				}
				else if (ianalysisService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 AnalysisService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					object_0 = ielementService_0.GetElementById(aitoolContext_0.Document, int_1);
					if (object_0 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到 ID 为 ");
						defaultInterpolatedStringHandler.AppendFormatted(int_1);
						defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
						result = AIToolResult.Fail(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						string_1 = ielementService_0.GetElementName(object_0);
						string_2 = ielementService_0.GetElementCategory(object_0);
						class164_0 = new Class164<int, string, string, string, string, string>(int_1, string_1, string_2, "millimeters", "square_meters", "cubic_meters");
						object_1 = class164_0;
						try
						{
							nullable_0 = igeometryService_0.GetBoundingBox(object_0);
							if (nullable_0.HasValue && nullable_0.Value.Min.HasValue && nullable_0.Value.Max.HasValue)
							{
								class165_0 = new Class165<_003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>>(new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(nullable_0.Value.Min.Value.MinX * 304.8, 0), Math.Round(nullable_0.Value.Min.Value.MinY * 304.8, 0), Math.Round(nullable_0.Value.Min.Value.MinZ * 304.8, 0)), new _003C_003Ef__AnonymousType27<double, double, double>(Math.Round(nullable_0.Value.Max.Value.MaxX * 304.8, 0), Math.Round(nullable_0.Value.Max.Value.MaxY * 304.8, 0), Math.Round(nullable_0.Value.Max.Value.MaxZ * 304.8, 0)));
								object_1 = new Class166<int, string, string, string, string, string, Class165<_003C_003Ef__AnonymousType27<double, double, double>, _003C_003Ef__AnonymousType27<double, double, double>>>(int_1, string_1, string_2, "millimeters", "square_meters", "cubic_meters", class165_0);
								class165_0 = null;
							}
						}
						catch
						{
						}
						try
						{
							nullable_1 = ianalysisService_0.GetElementLength(object_0);
							if (nullable_1.HasValue && nullable_1.Value > 0.0)
							{
								double_0 = nullable_1.Value * 304.8;
								if (Class496.callSite_0 == null)
								{
									Class496.callSite_0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementId", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_ = Class496.callSite_0.Target(Class496.callSite_0, object_1);
								if (Class496.callSite_1 == null)
								{
									Class496.callSite_1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementName", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_2 = Class496.callSite_1.Target(Class496.callSite_1, object_1);
								if (Class496.callSite_2 == null)
								{
									Class496.callSite_2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "category", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_3 = Class496.callSite_2.Target(Class496.callSite_2, object_1);
								if (Class496.callSite_3 == null)
								{
									Class496.callSite_3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "length_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_4 = Class496.callSite_3.Target(Class496.callSite_3, object_1);
								if (Class496.callSite_4 == null)
								{
									Class496.callSite_4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "area_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_5 = Class496.callSite_4.Target(Class496.callSite_4, object_1);
								if (Class496.callSite_5 == null)
								{
									Class496.callSite_5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "volume_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_6 = Class496.callSite_5.Target(Class496.callSite_5, object_1);
								if (Class496.callSite_6 == null)
								{
									Class496.callSite_6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "boundingBox", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object_1 = new Class167<object, object, object, object, object, object, object, Class168<double, double>>(gparam_, gparam_2, gparam_3, gparam_4, gparam_5, gparam_6, Class496.callSite_6.Target(Class496.callSite_6, object_1), new Class168<double, double>(Math.Round(double_0, 0), Math.Round(double_0 / 1000.0, 3)));
							}
						}
						catch
						{
						}
						try
						{
							nullable_2 = ianalysisService_0.GetElementArea(aitoolContext_0.Document, object_0, string_0);
							if (nullable_2 > 0.0)
							{
								nullable_3 = nullable_2 * 0.092903;
								if (Class496.callSite_7 == null)
								{
									Class496.callSite_7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementId", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_7 = Class496.callSite_7.Target(Class496.callSite_7, object_1);
								if (Class496.callSite_8 == null)
								{
									Class496.callSite_8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementName", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_8 = Class496.callSite_8.Target(Class496.callSite_8, object_1);
								if (Class496.callSite_9 == null)
								{
									Class496.callSite_9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "category", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_9 = Class496.callSite_9.Target(Class496.callSite_9, object_1);
								if (Class496.callSite_10 == null)
								{
									Class496.callSite_10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "length_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_10 = Class496.callSite_10.Target(Class496.callSite_10, object_1);
								if (Class496.callSite_11 == null)
								{
									Class496.callSite_11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "area_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_11 = Class496.callSite_11.Target(Class496.callSite_11, object_1);
								if (Class496.callSite_12 == null)
								{
									Class496.callSite_12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "volume_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_12 = Class496.callSite_12.Target(Class496.callSite_12, object_1);
								if (Class496.callSite_13 == null)
								{
									Class496.callSite_13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "boundingBox", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_13 = Class496.callSite_13.Target(Class496.callSite_13, object_1);
								if (Class496.callSite_14 == null)
								{
									Class496.callSite_14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "length", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object_1 = new Class169<object, object, object, object, object, object, object, object, Class170<string, double, double>>(gparam_7, gparam_8, gparam_9, gparam_10, gparam_11, gparam_12, gparam_13, Class496.callSite_14.Target(Class496.callSite_14, object_1), new Class170<string, double, double>(string_0, Math.Round((double)(decimal)nullable_3.Value, 2), Math.Round((double)(decimal)nullable_2.Value, 2)));
							}
						}
						catch
						{
						}
						try
						{
							nullable_4 = ianalysisService_0.GetElementVolume(aitoolContext_0.Document, object_0);
							if (nullable_4 > 0.0)
							{
								nullable_5 = nullable_4 * 0.0283168;
								if (Class496.callSite_15 == null)
								{
									Class496.callSite_15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementId", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_14 = Class496.callSite_15.Target(Class496.callSite_15, object_1);
								if (Class496.callSite_16 == null)
								{
									Class496.callSite_16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementName", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_15 = Class496.callSite_16.Target(Class496.callSite_16, object_1);
								if (Class496.callSite_17 == null)
								{
									Class496.callSite_17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "category", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_16 = Class496.callSite_17.Target(Class496.callSite_17, object_1);
								if (Class496.callSite_18 == null)
								{
									Class496.callSite_18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "length_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_17 = Class496.callSite_18.Target(Class496.callSite_18, object_1);
								if (Class496.callSite_19 == null)
								{
									Class496.callSite_19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "area_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_18 = Class496.callSite_19.Target(Class496.callSite_19, object_1);
								if (Class496.callSite_20 == null)
								{
									Class496.callSite_20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "volume_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_19 = Class496.callSite_20.Target(Class496.callSite_20, object_1);
								if (Class496.callSite_21 == null)
								{
									Class496.callSite_21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "boundingBox", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_20 = Class496.callSite_21.Target(Class496.callSite_21, object_1);
								if (Class496.callSite_22 == null)
								{
									Class496.callSite_22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "length", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object gparam_21 = Class496.callSite_22.Target(Class496.callSite_22, object_1);
								if (Class496.callSite_23 == null)
								{
									Class496.callSite_23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "area", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								object_1 = new Class171<object, object, object, object, object, object, object, object, object, Class172<double, double>>(gparam_14, gparam_15, gparam_16, gparam_17, gparam_18, gparam_19, gparam_20, gparam_21, Class496.callSite_23.Target(Class496.callSite_23, object_1), new Class172<double, double>(Math.Round((double)(decimal)nullable_5.Value, 3), Math.Round((double)(decimal)nullable_4.Value, 3)));
							}
						}
						catch
						{
						}
						try
						{
							if (bool_0)
							{
								object_2 = igeometryService_0.GetElementGeometry(object_0, aitoolContext_0.Document);
								if (object_2 != null)
								{
									if (Class496.callSite_24 == null)
									{
										Class496.callSite_24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementId", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_22 = Class496.callSite_24.Target(Class496.callSite_24, object_1);
									if (Class496.callSite_25 == null)
									{
										Class496.callSite_25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "elementName", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_23 = Class496.callSite_25.Target(Class496.callSite_25, object_1);
									if (Class496.callSite_26 == null)
									{
										Class496.callSite_26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "category", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_24 = Class496.callSite_26.Target(Class496.callSite_26, object_1);
									if (Class496.callSite_27 == null)
									{
										Class496.callSite_27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "length_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_25 = Class496.callSite_27.Target(Class496.callSite_27, object_1);
									if (Class496.callSite_28 == null)
									{
										Class496.callSite_28 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "area_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_26 = Class496.callSite_28.Target(Class496.callSite_28, object_1);
									if (Class496.callSite_29 == null)
									{
										Class496.callSite_29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "volume_unit", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_27 = Class496.callSite_29.Target(Class496.callSite_29, object_1);
									if (Class496.callSite_30 == null)
									{
										Class496.callSite_30 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "boundingBox", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_28 = Class496.callSite_30.Target(Class496.callSite_30, object_1);
									if (Class496.callSite_31 == null)
									{
										Class496.callSite_31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "length", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_29 = Class496.callSite_31.Target(Class496.callSite_31, object_1);
									if (Class496.callSite_32 == null)
									{
										Class496.callSite_32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "area", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object gparam_30 = Class496.callSite_32.Target(Class496.callSite_32, object_1);
									if (Class496.callSite_33 == null)
									{
										Class496.callSite_33 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "volume", typeof(GetElementGeometryTool), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									object_1 = new Class173<object, object, object, object, object, object, object, object, object, object, object>(gparam_22, gparam_23, gparam_24, gparam_25, gparam_26, gparam_27, gparam_28, gparam_29, gparam_30, Class496.callSite_33.Target(Class496.callSite_33, object_1), object_2);
								}
								object_2 = null;
							}
						}
						catch
						{
						}
						if (Class496.callSite_34 == null)
						{
							Class496.callSite_34 = CallSite<Func<CallSite, Type, string, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Ok", null, typeof(GetElementGeometryTool), new CSharpArgumentInfo[3]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						result = (AIToolResult)(dynamic)Class496.callSite_34.Target(Class496.callSite_34, typeof(AIToolResult), "成功获取元素几何信息: " + (string_1 ?? "未命名"), object_1);
					}
				}
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				result = AIToolResult.Fail("获取几何信息失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_element_geometry";

	public string Category => "几何分析";

	public string Description => "获取元素的完整几何信息，包括包围盒、长度、面积、体积、几何点等。返回单位：毫米、平方米、立方米";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"elementId\": {\r\n                \"type\": \"integer\",\r\n                \"description\": \"元素的 ID\"\r\n            },\r\n            \"areaType\": {\r\n                \"type\": \"string\",\r\n                \"description\": \"面积类型（surface=表面积, projection=投影面积, gross=总面积, net=净面积）\",\r\n                \"enum\": [\"surface\", \"projection\", \"gross\", \"net\"],\r\n                \"default\": \"surface\"\r\n            },\r\n            \"computeReferences\": {\r\n                \"type\": \"boolean\",\r\n                \"description\": \"是否计算几何引用（用于获取详细几何点信息）\",\r\n                \"default\": false\r\n            }\r\n        },\r\n        \"required\": [\"elementId\"]\r\n    }";

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class497))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class497 stateMachine = new Class497();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getElementGeometryTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
