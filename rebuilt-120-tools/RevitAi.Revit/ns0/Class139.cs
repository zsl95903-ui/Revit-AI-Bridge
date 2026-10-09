using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[DebuggerDisplay("\\{ total_elements = {total_elements}, total_categories = {total_categories}, categories = {categories} }", Type = "<Anonymous Type>")]
[CompilerGenerated]
internal sealed class Class139<T, U, V>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly V gparam_2;

	public T total_elements => gparam_0;

	public U total_categories => gparam_1;

	public V categories => gparam_2;

	[DebuggerHidden]
	public Class139(T gparam_3, U gparam_4, V gparam_5)
	{
		gparam_0 = gparam_3;
		gparam_1 = gparam_4;
		gparam_2 = gparam_5;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class139<T, U, V> @class = obj as Class139<T, U, V>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((-134489293 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ total_elements = {0}, total_categories = {1}, categories = {2} }}";
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
