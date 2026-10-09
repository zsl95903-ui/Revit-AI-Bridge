using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns7;

namespace ns0;

[CompilerGenerated]
internal sealed class Class44<T>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	public T p_user_id => gparam_0;

	[DebuggerHidden]
	public Class44(T gparam_1)
	{
		gparam_0 = gparam_1;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class44<T> @class = obj as Class44<T>;
		if (this != @class)
		{
			if (@class != null)
			{
				return EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0);
			}
			return false;
		}
		return true;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return 1522318100 + EqualityComparer<T>.Default.GetHashCode(gparam_0);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ p_user_id = {0} }}";
		object[] array = new object[1];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		return string.Format(null, format, array);
	}
}
