using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ viewId = {viewId}, viewName = {viewName}, direction = {direction} }", Type = "<Anonymous Type>")]
internal sealed class Class78<T, U, V>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly V gparam_2;

	public T viewId => gparam_0;

	public U viewName => gparam_1;

	public V direction => gparam_2;

	[DebuggerHidden]
	public Class78(T gparam_3, U gparam_4, V gparam_5)
	{
		gparam_0 = gparam_3;
		gparam_1 = gparam_4;
		gparam_2 = gparam_5;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class78<T, U, V> @class = obj as Class78<T, U, V>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((-400278349 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ viewId = {0}, viewName = {1}, direction = {2} }}";
		object[] array = new object[3];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		V val3 = gparam_2;
		array[2] = ((val3 != null) ? val3.ToString() : null);
		return string.Format(null, format, array);
	}
}
