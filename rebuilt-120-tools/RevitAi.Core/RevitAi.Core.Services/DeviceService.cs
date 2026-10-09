using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Core.Security;
using ns7;

namespace RevitAi.Core.Services;

public sealed class DeviceService : IDeviceService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct28 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public DeviceService deviceService_0;

		void IAsyncStateMachine.MoveNext()
		{
			DeviceService deviceService = deviceService_0;
			string result;
			if (!string.IsNullOrWhiteSpace(deviceService.string_0))
			{
				result = deviceService.string_0;
			}
			else
			{
				try
				{
					INativeCryptoService nativeCryptoService;
					try
					{
						nativeCryptoService = CoreServicesFactory.CreateCryptoService();
					}
					catch (InvalidOperationException ex)
					{
						Logger.Error("[DeviceService] 无法获取加密服务: " + ex.Message);
						result = null;
						goto end_IL_001e;
					}
					string hardwareFingerprintV = nativeCryptoService.GetHardwareFingerprintV2();
					if (string.IsNullOrWhiteSpace(hardwareFingerprintV))
					{
						Logger.Error("[DeviceService] 硬件指纹为空");
						result = null;
					}
					else
					{
						deviceService.string_0 = hardwareFingerprintV;
						result = deviceService.string_0;
					}
					end_IL_001e:;
				}
				catch (Exception ex2)
				{
					Logger.Error("[DeviceService] 获取设备 ID 失败: " + ex2.Message, ex2);
					result = null;
				}
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private string? string_0;

	[AsyncStateMachine(typeof(Struct28))]
	public Task<string?> GetDeviceIdAsync()
	{
		Struct28 stateMachine = default(Struct28);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.deviceService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
