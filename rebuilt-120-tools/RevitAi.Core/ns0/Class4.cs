using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns7;

namespace ns0;

[CompilerGenerated]
internal sealed class Class4<T, U>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	public T type => gparam_0;

	public U source => gparam_1;

	[DebuggerHidden]
	public Class4(T gparam_2, U gparam_3)
	{
		gparam_0 = gparam_2;
		gparam_1 = gparam_3;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class4<T, U> @class = obj as Class4<T, U>;
		if (this != @class)
		{
			if (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0))
			{
				return EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1);
			}
			return false;
		}
		return true;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (-482699737 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ type = {0}, source = {1} }}";
		object[] array = new object[2];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		return string.Format(null, format, array);
	}
}
