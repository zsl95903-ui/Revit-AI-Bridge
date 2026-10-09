using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ floor_id = {floor_id}, floor_name = {floor_name}, point_count = {point_count} }", Type = "<Anonymous Type>")]
internal sealed class Class57<T, U, V>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly V gparam_2;

	public T floor_id => gparam_0;

	public U floor_name => gparam_1;

	public V point_count => gparam_2;

	[DebuggerHidden]
	public Class57(T gparam_3, U gparam_4, V gparam_5)
	{
		gparam_0 = gparam_3;
		gparam_1 = gparam_4;
		gparam_2 = gparam_5;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class57<T, U, V> @class = obj as Class57<T, U, V>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((123940706 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ floor_id = {0}, floor_name = {1}, point_count = {2} }}";
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
