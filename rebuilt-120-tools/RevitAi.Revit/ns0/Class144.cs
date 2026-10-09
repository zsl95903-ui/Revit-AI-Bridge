using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[DebuggerDisplay("\\{ total_count = {total_count}, straight_grid_count = {straight_grid_count}, arc_grid_count = {arc_grid_count}, location_unit = {location_unit}, grids = {grids} }", Type = "<Anonymous Type>")]
[CompilerGenerated]
internal sealed class Class144<T, U, V, W, X>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T gparam_0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly U gparam_1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly V gparam_2;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly W gparam_3;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly X gparam_4;

	public T total_count => gparam_0;

	public U straight_grid_count => gparam_1;

	public V arc_grid_count => gparam_2;

	public W location_unit => gparam_3;

	public X grids => gparam_4;

	[DebuggerHidden]
	public Class144(T gparam_5, U gparam_6, V gparam_7, W gparam_8, X gparam_9)
	{
		gparam_0 = gparam_5;
		gparam_1 = gparam_6;
		gparam_2 = gparam_7;
		gparam_3 = gparam_8;
		gparam_4 = gparam_9;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class144<T, U, V, W, X> @class = obj as Class144<T, U, V, W, X>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((((1057316751 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ total_count = {0}, straight_grid_count = {1}, arc_grid_count = {2}, location_unit = {3}, grids = {4} }}";
		object[] array = new object[5];
		T val = gparam_0;
		array[0] = ((val != null) ? val.ToString() : null);
		U val2 = gparam_1;
		array[1] = ((val2 != null) ? val2.ToString() : null);
		V val3 = gparam_2;
		array[2] = ((val3 != null) ? val3.ToString() : null);
		W val4 = gparam_3;
		array[3] = ((val4 != null) ? val4.ToString() : null);
		X val5 = gparam_4;
		array[4] = ((val5 != null) ? val5.ToString() : null);
		return string.Format(null, format, array);
	}
}
