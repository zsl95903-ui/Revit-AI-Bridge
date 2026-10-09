using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ elevation = {elevation} }", Type = "<Anonymous Type>")]
internal sealed class Class81<T>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	public T elevation => gparam_0;

	[DebuggerHidden]
	public Class81(T gparam_1)
	{
		gparam_0 = gparam_1;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class81<T> @class = obj as Class81<T>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return -1395080801 + EqualityComparer<T>.Default.GetHashCode(gparam_0);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ elevation = {0} }}";
		object[] array = new object[1];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		return string.Format(null, format, array);
	}
}
