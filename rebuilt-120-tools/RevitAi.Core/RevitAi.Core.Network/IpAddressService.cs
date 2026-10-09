using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Network;
using ns7;

namespace RevitAi.Core.Network;

public sealed class IpAddressService
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct37 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<(string? PublicIp, string? LocalIp)> asyncTaskMethodBuilder_0;

		public IpAddressService ipAddressService_0;

		private TaskAwaiter<string?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			IpAddressService ipAddressService = ipAddressService_0;
			TaskAwaiter<string> awaiter;
			if (num != 0)
			{
				awaiter = ipAddressService.GetPublicIpAddressAsync().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter<string>);
				num = -1;
				int_0 = -1;
			}
			string result = awaiter.GetResult();
			string localIpAddress = ipAddressService.GetLocalIpAddress();
			(string, string) result2 = (result, localIpAddress);
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct38 : IAsyncStateMachine
	{
			private string requestUri;
		public int int_0;

		public AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder_0;

		public IpAddressService ipAddressService_0;

		private string[] string_0;

		private int int_1;

		private TaskAwaiter<HttpResponseMessage> taskAwaiter_0;

		private TaskAwaiter<string> taskAwaiter_1;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			IpAddressService ipAddressService = ipAddressService_0;
			if ((uint)num <= 1u)
			{
				goto IL_0061;
			}
			string[] array = new string[4]
			{
				"https://api.ipify.org",
				"https://icanhazip.com",
				"https://ifconfig.me/ip",
				"http://checkip.amazonaws.com"
			};
			string_0 = array;
			int_1 = 0;
			goto IL_0155;
			IL_0194:
			int_0 = -2;
			string result;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_0155:
			if (int_1 < string_0.Length)
			{
				requestUri = string_0[int_1];
				goto IL_0061;
			}
			string_0 = null;
			result = null;
			goto IL_0194;
			IL_0061:
			try
			{
				TaskAwaiter<string> awaiter;
				TaskAwaiter<HttpResponseMessage> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = taskAwaiter_1;
						taskAwaiter_1 = default(TaskAwaiter<string>);
						num = -1;
						int_0 = -1;
						goto IL_0134;
					}
					awaiter2 = ipAddressService.httpClient_0.GetAsync(requestUri).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter2;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<HttpResponseMessage>);
					num = -1;
					int_0 = -1;
				}
				HttpResponseMessage result2 = awaiter2.GetResult();
				if (result2.IsSuccessStatusCode)
				{
					awaiter = result2.Content.ReadAsStringAsync().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_1 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0134;
				}
				goto end_IL_0061;
				IL_0134:
				string text = awaiter.GetResult().Trim();
				if (smethod_0(text))
				{
					result = text;
					goto IL_0194;
				}
				end_IL_0061:;
			}
			catch
			{
			}
			int_1++;
			goto IL_0155;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly HttpClient httpClient_0;

	private const int int_0 = 3;

	public IpAddressService(IHttpClientFactory? httpClientFactory = null)
	{
		if (httpClientFactory != null)
		{
			httpClient_0 = httpClientFactory.CreateClient((string)null, 3);
			return;
		}
		httpClient_0 = new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(3L)
		};
	}

	[AsyncStateMachine(typeof(Struct38))]
	public Task<string?> GetPublicIpAddressAsync()
	{
		Struct38 stateMachine = default(Struct38);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<string>.Create();
		stateMachine.ipAddressService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	public string? GetLocalIpAddress()
	{
		try
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			IPAddress[] array = hostAddresses;
			int num = 0;
			IPAddress iPAddress;
			while (true)
			{
				if (num < array.Length)
				{
					iPAddress = array[num];
					if (!IPAddress.IsLoopback(iPAddress) && iPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						break;
					}
					num++;
					continue;
				}
				array = hostAddresses;
				num = 0;
				IPAddress iPAddress2;
				while (true)
				{
					if (num < array.Length)
					{
						iPAddress2 = array[num];
						if (!IPAddress.IsLoopback(iPAddress2) && iPAddress2.AddressFamily == AddressFamily.InterNetworkV6)
						{
							break;
						}
						num++;
						continue;
					}
					return null;
				}
				return iPAddress2.ToString();
			}
			return iPAddress.ToString();
		}
		catch
		{
			return null;
		}
	}

	private static bool smethod_0(string string_0)
	{
		if (string.IsNullOrWhiteSpace(string_0))
		{
			return false;
		}
		string_0 = string_0.Trim();
		IPAddress address;
		return IPAddress.TryParse(string_0, out address);
	}

	[AsyncStateMachine(typeof(Struct37))]
	public Task<(string? PublicIp, string? LocalIp)> GetIpAddressesAsync()
	{
		Struct37 stateMachine = default(Struct37);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<(string, string)>.Create();
		stateMachine.ipAddressService_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
