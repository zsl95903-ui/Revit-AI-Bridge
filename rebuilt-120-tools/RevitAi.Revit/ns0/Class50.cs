using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ duct_id = {duct_id}, duct_name = {duct_name}, start_x = {start_x}, start_y = {start_y}, start_z = {start_z}, end_x = {end_x}, end_y = {end_y}, end_z = {end_z}, width = {width}, height = {height} ... }", Type = "<Anonymous Type>")]
internal sealed class Class50<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11, T12>
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

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Y gparam_5;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Z gparam_6;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T7 gparam_7;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T8 gparam_8;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T9 gparam_9;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T10 gparam_10;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T11 gparam_11;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly T12 gparam_12;

	public T duct_id => gparam_0;

	public U duct_name => gparam_1;

	public V start_x => gparam_2;

	public W start_y => gparam_3;

	public X start_z => gparam_4;

	public Y end_x => gparam_5;

	public Z end_y => gparam_6;

	public T7 end_z => gparam_7;

	public T8 width => gparam_8;

	public T9 height => gparam_9;

	public T10 diameter => gparam_10;

	public T11 shape => gparam_11;

	public T12 system_type_id => gparam_12;

	[DebuggerHidden]
	public Class50(T gparam_13, U gparam_14, V gparam_15, W gparam_16, X gparam_17, Y gparam_18, Z gparam_19, T7 gparam_20, T8 gparam_21, T9 gparam_22, T10 gparam_23, T11 gparam_24, T12 gparam_25)
	{
		gparam_0 = gparam_13;
		gparam_1 = gparam_14;
		gparam_2 = gparam_15;
		gparam_3 = gparam_16;
		gparam_4 = gparam_17;
		gparam_5 = gparam_18;
		gparam_6 = gparam_19;
		gparam_7 = gparam_20;
		gparam_8 = gparam_21;
		gparam_9 = gparam_22;
		gparam_10 = gparam_23;
		gparam_11 = gparam_24;
		gparam_12 = gparam_25;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class50<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11, T12> @class = obj as Class50<T, U, V, W, X, Y, Z, T7, T8, T9, T10, T11, T12>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6) && EqualityComparer<T7>.Default.Equals(gparam_7, @class.gparam_7) && EqualityComparer<T8>.Default.Equals(gparam_8, @class.gparam_8) && EqualityComparer<T9>.Default.Equals(gparam_9, @class.gparam_9) && EqualityComparer<T10>.Default.Equals(gparam_10, @class.gparam_10) && EqualityComparer<T11>.Default.Equals(gparam_11, @class.gparam_11) && EqualityComparer<T12>.Default.Equals(gparam_12, @class.gparam_12));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((((((((((((-362217230 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6)) * -1521134295 + EqualityComparer<T7>.Default.GetHashCode(gparam_7)) * -1521134295 + EqualityComparer<T8>.Default.GetHashCode(gparam_8)) * -1521134295 + EqualityComparer<T9>.Default.GetHashCode(gparam_9)) * -1521134295 + EqualityComparer<T10>.Default.GetHashCode(gparam_10)) * -1521134295 + EqualityComparer<T11>.Default.GetHashCode(gparam_11)) * -1521134295 + EqualityComparer<T12>.Default.GetHashCode(gparam_12);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ duct_id = {0}, duct_name = {1}, start_x = {2}, start_y = {3}, start_z = {4}, end_x = {5}, end_y = {6}, end_z = {7}, width = {8}, height = {9}, diameter = {10}, shape = {11}, system_type_id = {12} }}";
		object[] array = new object[13];
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
		Y val6 = gparam_5;
		array[5] = ((val6 != null) ? val6.ToString() : null);
		Z val7 = gparam_6;
		array[6] = ((val7 != null) ? val7.ToString() : null);
		T7 val8 = gparam_7;
		array[7] = ((val8 != null) ? val8.ToString() : null);
		T8 val9 = gparam_8;
		array[8] = ((val9 != null) ? val9.ToString() : null);
		T9 val10 = gparam_9;
		array[9] = ((val10 != null) ? val10.ToString() : null);
		T10 val11 = gparam_10;
		array[10] = ((val11 != null) ? val11.ToString() : null);
		T11 val12 = gparam_11;
		array[11] = ((val12 != null) ? val12.ToString() : null);
		T12 val13 = gparam_12;
		array[12] = ((val13 != null) ? val13.ToString() : null);
		return string.Format(null, format, array);
	}
}
