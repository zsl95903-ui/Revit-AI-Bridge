using System;

namespace ns1;

internal class Class73
{
	public class Class74 : IDisposable
	{
		private string string_0;

		private string string_1;

		private string string_2;

		public string Password => string_2;

		public Class74(string string_3, string string_4, string string_5)
		{
			string_0 = string_3;
			string_1 = string_4;
			string_2 = string_5;
		}

		public Class74(string string_3)
		{
			string_0 = string_3;
			string_1 = Environment.UserName;
			string_2 = string.Empty;
		}

		public void method_0()
		{
		}

		void IDisposable.Dispose()
		{
		}
	}
}
