using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ns6;

namespace ns0;

[CompilerGenerated]
[DebuggerDisplay("\\{ element_id = {element_id}, element_type = {element_type}, width_mm = {width_mm}, height_mm = {height_mm}, diameter_mm = {diameter_mm}, system_type_id = {system_type_id}, length_feet = {length_feet}, length_mm = {length_mm} }", Type = "<Anonymous Type>")]
internal sealed class Class228<T, U, V, W, X, Y, Z, T7>
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

	public T element_id => gparam_0;

	public U element_type => gparam_1;

	public V width_mm => gparam_2;

	public W height_mm => gparam_3;

	public X diameter_mm => gparam_4;

	public Y system_type_id => gparam_5;

	public Z length_feet => gparam_6;

	public T7 length_mm => gparam_7;

	[DebuggerHidden]
	public Class228(T gparam_8, U gparam_9, V gparam_10, W gparam_11, X gparam_12, Y gparam_13, Z gparam_14, T7 gparam_15)
	{
		gparam_0 = gparam_8;
		gparam_1 = gparam_9;
		gparam_2 = gparam_10;
		gparam_3 = gparam_11;
		gparam_4 = gparam_12;
		gparam_5 = gparam_13;
		gparam_6 = gparam_14;
		gparam_7 = gparam_15;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		Class228<T, U, V, W, X, Y, Z, T7> @class = obj as Class228<T, U, V, W, X, Y, Z, T7>;
		return this == @class || (@class != null && EqualityComparer<T>.Default.Equals(gparam_0, @class.gparam_0) && EqualityComparer<U>.Default.Equals(gparam_1, @class.gparam_1) && EqualityComparer<V>.Default.Equals(gparam_2, @class.gparam_2) && EqualityComparer<W>.Default.Equals(gparam_3, @class.gparam_3) && EqualityComparer<X>.Default.Equals(gparam_4, @class.gparam_4) && EqualityComparer<Y>.Default.Equals(gparam_5, @class.gparam_5) && EqualityComparer<Z>.Default.Equals(gparam_6, @class.gparam_6) && EqualityComparer<T7>.Default.Equals(gparam_7, @class.gparam_7));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (((((((758408422 + EqualityComparer<T>.Default.GetHashCode(gparam_0)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(gparam_1)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(gparam_2)) * -1521134295 + EqualityComparer<W>.Default.GetHashCode(gparam_3)) * -1521134295 + EqualityComparer<X>.Default.GetHashCode(gparam_4)) * -1521134295 + EqualityComparer<Y>.Default.GetHashCode(gparam_5)) * -1521134295 + EqualityComparer<Z>.Default.GetHashCode(gparam_6)) * -1521134295 + EqualityComparer<T7>.Default.GetHashCode(gparam_7);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string format = "{{ element_id = {0}, element_type = {1}, width_mm = {2}, height_mm = {3}, diameter_mm = {4}, system_type_id = {5}, length_feet = {6}, length_mm = {7} }}";
		object[] array = new object[8];
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
		return string.Format(null, format, array);
	}
}
