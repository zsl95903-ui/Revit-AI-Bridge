using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ system_type_id = {system_type_id}, system_name = {system_name}, element_type = {element_type}, size_ranges = {size_ranges} }", Type = "<Anonymous Type>")]
internal sealed class Class220<T, U, V, W>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly V gparam_2;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly W gparam_3;

	public T system_type_id => gparam_0;

	public U system_name => gparam_1;

	public V element_type => gparam_2;

	public W size_ranges => gparam_3;

	[DebuggerHidden]
	public Class220(T gparam_4, U gparam_5, V gparam_6, W gparam_7)
	{
		gparam_0 = gparam_4;
		gparam_1 = gparam_5;
		gparam_2 = gparam_6;
		gparam_3 = gparam_7;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class220<T, U, V, W> @class = obj as Class220<T, U, V, W>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (((21318460 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ system_type_id = {0}, system_name = {1}, element_type = {2}, size_ranges = {3} }}";
		object[] array = new object[4];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		V val3 = gparam_2;
		array[2] = ((val3 != null) ? val3.ToString() : null);
		W val4 = gparam_3;
		array[3] = ((val4 != null) ? val4.ToString() : null);
		return string.Format(null, format, array);
	}
}
